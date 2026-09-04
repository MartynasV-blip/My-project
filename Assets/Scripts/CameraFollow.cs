using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -2.5f);
    public float followSpeed = 15f;
    public float lookHeight = 1f;

    void LateUpdate() {
        if (target == null) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSpeed * Time.deltaTime));
        transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * lookHeight - transform.position);
    }
}