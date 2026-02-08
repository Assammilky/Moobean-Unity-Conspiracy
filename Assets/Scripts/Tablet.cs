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
    private Rigidbody centerPieceRB;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        centerPieceRB = centerPiece.GetComponent<Rigidbody>();
        //centerPiece.SetParent(centerPiece.parent, true);
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
        Vector3 originalRotation = transform.localEulerAngles;
        float lastAngle = 0;
        Debug.Log(originalRotation);
        for(float timer = 0; timer < 1; timer += Time.fixedDeltaTime / rotateTime)
        {
            float easeOut = Mathf.Sin(timer * Mathf.PI * 0.5f);
            float magnitude = easeOut - lastAngle;
            float angleChange = 180f / rotateTime * magnitude;
            Quaternion localRotation = Quaternion.Euler(angleChange * direction.x, 0, angleChange * direction.z);
            centerPieceRB.MoveRotation(centerPieceRB.rotation * localRotation);

            lastAngle = easeOut;
            yield return new WaitForFixedUpdate();
        }
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


    private Vector3 RoundedVector(Vector3 vector, float roundBy)
    {
        vector.x = Mathf.Round(vector.x / roundBy) * roundBy;
        vector.y = Mathf.Round(vector.y / roundBy) * roundBy;
        vector.z = Mathf.Round(vector.z / roundBy) * roundBy;
        return vector;
    }
}
