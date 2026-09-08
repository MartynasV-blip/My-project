using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [Header("Timing")]
    public float ragdollDuration = 5f;
    public float standUpHeight = 1f;

    [Header("Death")]
    public bool dieAfterRagdoll = false;
    public float fadeDuration = 1f;

    [Header("Ground Check")]
    public LayerMask groundLayers = ~0;

    private Animator animator;
    private Rigidbody rootBody;
    private Collider rootCollider;

    private Rigidbody[] boneBodies;
    private Collider[] boneColliders;
    private Transform pelvis;

    private bool isRagdolled;
    public bool IsRagdolled => isRagdolled;

    public float RagdollTimeRemaining { get; private set; }
    public event Action<float> OnRagdollTimeChanged;
    public event Action OnRagdollStart;
    public event Action OnRagdollEnd;

    void Awake() {
        animator = GetComponentInChildren<Animator>();
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

            if (!b.isKinematic) {
                b.linearVelocity = Vector3.zero;
                b.angularVelocity = Vector3.zero;
            }

            b.isKinematic = !physicsOn;
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
        OnRagdollStart?.Invoke();

        Rigidbody target = FindNearestBone(hitPoint);
        if (target != null) target.AddForce(force, ForceMode.Impulse);

        StartCoroutine(StandUpAfterDelay());
    }

    IEnumerator StandUpAfterDelay() {
        RagdollTimeRemaining = ragdollDuration;

        while (RagdollTimeRemaining > 0f) {
            OnRagdollTimeChanged?.Invoke(RagdollTimeRemaining);
            yield return null;
            RagdollTimeRemaining -= Time.deltaTime;
        }

        RagdollTimeRemaining = 0f;
        OnRagdollTimeChanged?.Invoke(0f);

        if (dieAfterRagdoll) {
            yield return StartCoroutine(FadeAndDie());
        } else {
            StandUp();
            OnRagdollEnd?.Invoke();
        }
    }

    IEnumerator FadeAndDie() {
        var renderers = GetComponentsInChildren<Renderer>();
        var mats = new List<Material>();

        foreach (Renderer r in renderers)
            foreach (Material m in r.materials) {
                MakeTransparent(m);
                mats.Add(m);
            }

        float t = 0f;
        while (t < fadeDuration) {
            float a = 1f - (t / fadeDuration);

            foreach (Material m in mats) {
                if (m.HasProperty("_BaseColor")) {
                    Color c = m.GetColor("_BaseColor");
                    c.a = a;
                    m.SetColor("_BaseColor", c);
                } else if (m.HasProperty("_Color")) {
                    Color c = m.color;
                    c.a = a;
                    m.color = c;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void MakeTransparent(Material m) {
        m.SetFloat("_Surface", 1f);
        m.SetFloat("_Blend", 0f);
        m.SetOverrideTag("RenderType", "Transparent");
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
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

        if (rootBody != null && !rootBody.isKinematic) {
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