using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveForce = 10f;
    public float turnSpeed = 360f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 input;
    private Animator anim;

    void Start() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        if (rb == null)
            Debug.LogError($"No Rigidbody attached to {name}. PlayerMovement requires a Rigidbody component.");
    }

    void Update() {
        var kb = Keyboard.current;
        if (kb == null || cameraTransform == null) { input = Vector3.zero; return; }

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

    void FixedUpdate() {
        if (rb == null || input == Vector3.zero) return;

        rb.AddForce(input * moveForce);

        Quaternion target = Quaternion.LookRotation(input, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
    }
}