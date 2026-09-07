using UnityEngine;

public class RagdollImpact : MonoBehaviour
{
    [Header("Impact")]
    public float minImpulse = 4f;
    public float passOnForce = 12f;
    public float passOnLift = 1.5f;

    [Header("Limits")]
    public float perTargetCooldown = 0.5f;

    private Ragdoll ownRagdoll;
    private float lastPassOn = -999f;

    void Awake() {
        ownRagdoll = GetComponentInParent<Ragdoll>();
        if (ownRagdoll == null)
            Debug.LogError($"{name}: RagdollImpact needs a Ragdoll component on a parent object.");
    }

    void OnCollisionEnter(Collision collision) {
        if (ownRagdoll == null || !ownRagdoll.IsRagdolled) return;

        if (Time.time < lastPassOn + perTargetCooldown) return;

        if (collision.impulse.magnitude < minImpulse) return;

        Ragdoll other = collision.collider.GetComponentInParent<Ragdoll>();
        if (other == null || other == ownRagdoll) return;
        if (other.IsRagdolled) return;

        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) dir = transform.forward;
        dir.Normalize();

        Vector3 contactPoint = collision.contacts.Length > 0
            ? collision.contacts[0].point
            : transform.position;

        other.Hit(dir * passOnForce + Vector3.up * passOnLift, contactPoint);
        lastPassOn = Time.time;

        Debug.Log($"{ownRagdoll.name} knocked down {other.name}");
    }
}