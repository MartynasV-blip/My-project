using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 10f;
    public float turnSpeed = 360f;
    public Transform cameraTransform;

    [Header("Punch")]
    public float punchCooldown = 0.6f;
    public Transform punchPoint;
    public float punchRadius = 0.4f;
    public float punchWindowStart = 0.15f;
    public float punchWindowDuration = 0.25f;
    public LayerMask hittableLayers;
    public float punchForce = 8f;
    public float punchLift = 2f;
    public float ragdollForce = 25f;

    private Rigidbody rb;
    private Vector3 input;
    private Animator anim;
    private float lastPunchTime = -999f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        if (rb == null)
            Debug.LogError($"No Rigidbody attached to {name}. PlayerMovement requires a Rigidbody component.");
    }

    void Update() {
        var kb = Keyboard.current;
        if (kb == null || cameraTransform == null) { input = Vector3.zero; return; }

        if (kb.spaceKey.wasPressedThisFrame && anim != null
            && Time.time >= lastPunchTime + punchCooldown) {
            anim.SetTrigger("Punch");
            lastPunchTime = Time.time;
            StartCoroutine(PunchHitCheck());
        }

        float h = 0f, v = 0f;
        if (kb.wKey.isPressed) v += 1f;
        if (kb.sKey.isPressed) v -= 1f;
        if (kb.dKey.isPressed) h += 1f;
        if (kb.aKey.isPressed) h -= 1f;

        Vector3 fwd = cameraTransform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = cameraTransform.right; right.y = 0f; right.Normalize();

        input = (fwd * v + right * h).normalized;

        if (anim != null && rb != null) {
            Vector3 flat = rb.linearVelocity;
            flat.y = 0f;
            anim.SetFloat("Speed", flat.magnitude, 0.1f, Time.deltaTime);
        }
    }

    IEnumerator PunchHitCheck() {
        if (punchPoint == null) {
            Debug.LogWarning("PunchPoint is not assigned on PlayerMovement.");
            yield break;
        }

        yield return new WaitForSeconds(punchWindowStart);

        float elapsed = 0f;
        var alreadyHit = new HashSet<Collider>();
        var alreadyRagdolled = new HashSet<Ragdoll>();

        while (elapsed < punchWindowDuration) {
            Collider[] hits = Physics.OverlapSphere(punchPoint.position, punchRadius, hittableLayers);

            foreach (Collider c in hits) {
                if (alreadyHit.Contains(c)) continue;
                alreadyHit.Add(c);

                Debug.Log($"Punch hit: {c.name}");

                Vector3 dir = c.transform.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.001f) dir = transform.forward;
                dir.Normalize();

                Ragdoll rag = c.GetComponentInParent<Ragdoll>();
                if (rag != null) {
                    if (alreadyRagdolled.Contains(rag)) continue;
                    alreadyRagdolled.Add(rag);
                    rag.Hit(dir * ragdollForce + Vector3.up * punchLift, punchPoint.position);
                    continue;
                }

                Rigidbody hitRb = c.attachedRigidbody;
                if (hitRb != null)
                    hitRb.AddForce(dir * punchForce + Vector3.up * punchLift, ForceMode.Impulse);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void FixedUpdate() {
        if (rb == null || input == Vector3.zero) return;

        rb.AddForce(input * moveForce);

        Quaternion target = Quaternion.LookRotation(input, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
    }

    void OnDrawGizmosSelected() {
        if (punchPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
    }
}