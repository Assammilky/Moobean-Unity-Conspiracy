using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private InputManager inputManager;
    private Transform cameraTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        if (inputManager == null || cameraTransform == null) return;

        Vector2 input = inputManager.GetMovementInput();

        // If input is too small, don’t move
        if (input.sqrMagnitude < 0.01f)
        {
            // Stop horizontal movement but keep gravity
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        // Camera-relative movement
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = camForward * input.y + camRight * input.x;
        move.Normalize();

        // Set Rigidbody velocity
        rb.velocity = move * moveSpeed + Vector3.up * rb.velocity.y;
    }
}

