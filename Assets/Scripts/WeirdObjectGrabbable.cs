using UnityEngine;

public class WeirdObjectGrabbable : MonoBehaviour, IGrabbable
{
    [SerializeField] private float lerpSpeed = 8f;
    [SerializeField] private float slerpSpeed = 5f;

    private Transform cameraTransform;
    private Rigidbody rb;
    private Transform grabPoint;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isGrabbed;
    private bool isReturning;

    private Quaternion targetRotation;

    private Quaternion meshOffset = Quaternion.Euler(0, -90, 0);
    private bool horizontalLock = false;
    private bool verticalLock = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPointTransform)
    {
        grabPoint = grabPointTransform;
        cameraTransform = Camera.main.transform;

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Initialize targetRotation to face camera + mesh offset
        targetRotation = Quaternion.LookRotation(cameraTransform.forward) * meshOffset;

        isGrabbed = true;
        isReturning = false;
    }

    public void Release()
    {
        isGrabbed = false;
        isReturning = true;
        grabPoint = null;
    }

    private void Update()
    {
        if (!isGrabbed) return;

        // Toggle horizontalLock when A or D is pressed
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            horizontalLock = !horizontalLock;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            verticalLock = !verticalLock;

        // Only allow W/S flips if horizontalLock is false
        if (!horizontalLock)
        {
            if (Input.GetKeyDown(KeyCode.W))
                targetRotation *= Quaternion.Euler(0f, 0f, 180f);
            if (Input.GetKeyDown(KeyCode.S))
                targetRotation *= Quaternion.Euler(0f, 0f, -180f);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
                targetRotation *= Quaternion.Euler(0f, 0f, -180f);
            if (Input.GetKeyDown(KeyCode.S))
                targetRotation *= Quaternion.Euler(0f, 0f, 180f);
        }

        if (!verticalLock)
        {
            if (Input.GetKeyDown(KeyCode.A))
                targetRotation *= Quaternion.Euler(0f, -180f, 0f);
            if (Input.GetKeyDown(KeyCode.D))
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A))
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
            if (Input.GetKeyDown(KeyCode.D))
                targetRotation *= Quaternion.Euler(0f, -180f, 0f);
        }

        // A/D rotation always works
        
    }

    private void FixedUpdate()
    {
        if (isGrabbed && grabPoint != null)
        {
            rb.MovePosition(Vector3.Lerp(rb.position, grabPoint.position, Time.fixedDeltaTime * lerpSpeed));
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, Time.fixedDeltaTime * slerpSpeed * 100f));
        }
        else if (isReturning)
        {
            rb.MovePosition(Vector3.Lerp(rb.position, originalPosition, Time.fixedDeltaTime * lerpSpeed));
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, originalRotation, Time.fixedDeltaTime * slerpSpeed));

            if (Vector3.Distance(rb.position, originalPosition) < 0.05f &&
                Quaternion.Angle(rb.rotation, originalRotation) < 1f)
            {
                isReturning = false;
            }
        }
    }
}
