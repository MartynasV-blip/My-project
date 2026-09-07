using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Ragdoll targetRagdoll;
    public Transform ragdollFollowBone;

    [Header("Normal view")]
    public Vector3 offset = new Vector3(0f, 2.5f, -5f);
    public float followSpeed = 15f;
    public float lookHeight = 1f;

    [Header("Ragdoll view")]
    public Vector3 ragdollOffset = new Vector3(0f, 6f, -0.5f);
    public float ragdollFollowSpeed = 4f;
    public float ragdollLookHeight = 0f;

    void LateUpdate() {
        bool down = targetRagdoll != null && targetRagdoll.IsRagdolled;

        Transform follow = target;
        if (down && ragdollFollowBone != null) follow = ragdollFollowBone;
        if (follow == null) return;

        Vector3 useOffset = down ? ragdollOffset : offset;
        float useSpeed = down ? ragdollFollowSpeed : followSpeed;
        float useLook = down ? ragdollLookHeight : lookHeight;

        Vector3 desired = follow.position + useOffset;
        transform.position = Vector3.Lerp(transform.position, desired,
            1f - Mathf.Exp(-useSpeed * Time.deltaTime));

        Vector3 lookAt = follow.position + Vector3.up * useLook;
        Vector3 dir = lookAt - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), 1f - Mathf.Exp(-useSpeed * Time.deltaTime));
    }
}