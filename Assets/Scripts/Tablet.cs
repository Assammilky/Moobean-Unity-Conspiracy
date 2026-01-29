using System.Collections;
using UnityEngine;

public class Tablet : MonoBehaviour, IGrabbable
{
    [SerializeField] private float lerpSpeed = 8f;
    [SerializeField] private float slerpSpeed = 5f;
    [SerializeField] private float rotateTime = 5f;

    private Transform cameraTransform;
    private Rigidbody rb;
    private Transform grabPoint;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isGrabbed;
    private bool isReturning;

    private Quaternion targetRotation;

    private Quaternion meshOffset = Quaternion.Euler(0, -90, 0);
    private bool rotationLock = false;

    [SerializeField] private Transform centerPiece;

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
        targetRotation = Quaternion.LookRotation(cameraTransform.up) * meshOffset;

        isGrabbed = true;
        isReturning = false;
    }

    public void Release()
    {
        isGrabbed = false;
        isReturning = true;
        grabPoint = null;
    }

    private IEnumerator RotateTablet(Vector3 direction)
    {
        HintsManager.inst.StartRotateTablet(direction, direction.x != 0);
        rotationLock = true;
        Vector3 originalRotation = centerPiece.localRotation.eulerAngles;
        for(float timer = 0; timer < 1; timer += Time.deltaTime / rotateTime)
        {
            float easeOut = Mathf.Sin(timer * Mathf.PI * 0.5f);
            if(direction.x == 0)
            {
                centerPiece.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(originalRotation.z, originalRotation.z - direction.z * 180, easeOut));
            }
            else
            {
                centerPiece.localRotation = Quaternion.Euler(Mathf.Lerp(originalRotation.x, originalRotation.x - direction.x * 180, easeOut), 0, 0);
            }
            // centerPiece.localRotation = Quaternion.Euler(Mathf.Lerp(originalRotation.x, originalRotation.x - direction.x * 180, easeOut), 0, Mathf.Lerp(originalRotation.z, originalRotation.z - direction.z * 180, easeOut));
            yield return null;
        }
       // centerPiece.localRotation = Quaternion.Euler(originalRotation.x - direction.x * 180,  0, originalRotation.z - direction.z * 180);
        rotationLock = false;
        HintsManager.inst.EndRotateTablet(direction, direction.x != 0);
    }

    private void Update()
    {
        if (!isGrabbed || rotationLock) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(RotateTablet(new Vector3(0, 0, -1)));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(RotateTablet(new Vector3(-1, 0, 0)));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(RotateTablet(new Vector3(0, 0, 1)));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(RotateTablet(new Vector3(1, 0, 0)));
        }        
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
