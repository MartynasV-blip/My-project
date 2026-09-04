using System.Collections;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [Header("Timing")]
    public float ragdollDuration = 10f;
    public float standUpHeight = 1f;

    [Header("Ground Check")]
    public LayerMask groundLayers = ~0;

    private Animator animator;
    private Rigidbody rootBody;
    private Collider rootCollider;

    private Rigidbody[] boneBodies;
    private Collider[] boneColliders;
    private Transform pelvis;

    private bool isRagdolled;

    void Awake() {
        animator = GetComponent<Animator>();
        rootBody = GetComponent<Rigidbody>();
        rootCollider = GetComponent<Collider>();

        boneBodies = GetComponentsInChildren<Rigidbody>();
        boneColliders = GetComponentsInChildren<Collider>();

        foreach (Rigidbody b in boneBodies) {
            if (b == rootBody) continue;
            if (b.name.Contains("Pelvis")) pelvis = b.transform;
        }

        if (pelvis == null)
            Debug.LogError($"{name}: could not find a pelvis bone. Ragdoll needs a bone with 'Pelvis' in its name.");

        SetBonesActive(false);
    }

    void SetBonesActive(bool physicsOn) {
        foreach (Rigidbody b in boneBodies) {
            if (b == rootBody) continue;
            b.isKinematic = !physicsOn;
            b.linearVelocity = Vector3.zero;
            b.angularVelocity = Vector3.zero;
        }

        foreach (Collider c in boneColliders) {
            if (c == rootCollider) continue;
            c.enabled = physicsOn;
        }

        if (animator != null) animator.enabled = !physicsOn;
        if (rootCollider != null) rootCollider.enabled = !physicsOn;

        isRagdolled = physicsOn;
    }

    public void Hit(Vector3 force, Vector3 hitPoint) {
        if (isRagdolled) return;

        SetBonesActive(true);

        Rigidbody target = FindNearestBone(hitPoint);
        if (target != null) target.AddForce(force, ForceMode.Impulse);

        StartCoroutine(StandUpAfterDelay());
    }

    IEnumerator StandUpAfterDelay() {
        yield return new WaitForSeconds(ragdollDuration);
        StandUp();
    }

    void StandUp() {
        if (pelvis == null) { SetBonesActive(false); return; }

        Vector3 landedPos = pelvis.position;

        Vector3 flatForward = pelvis.up;
        flatForward.y = 0f;
        if (flatForward.sqrMagnitude < 0.001f) flatForward = Vector3.forward;
        flatForward.Normalize();

        float groundY = landedPos.y;
        if (Physics.Raycast(landedPos + Vector3.up * 2f, Vector3.down,
                            out RaycastHit hit, 10f, groundLayers))
            groundY = hit.point.y;

        SetBonesActive(false);

        transform.position = new Vector3(landedPos.x, groundY + standUpHeight, landedPos.z);
        transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);

        if (rootBody != null) {
            rootBody.linearVelocity = Vector3.zero;
            rootBody.angularVelocity = Vector3.zero;
        }
    }

        Rigidbody FindNearestBone(Vector3 point) {
        Rigidbody best = null;
        float bestDist = float.MaxValue;

        foreach (Rigidbody b in boneBodies) {
            if (b == rootBody) continue;
            float d = (b.worldCenterOfMass - point).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = b; }
        }
        return best;
    }
}