using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 8f;
    [SerializeField] private float slerpSpeed = 2f;
    [SerializeField] private float rotationSpeed = 100f;

    private Transform cameraTransform;

    private Rigidbody rb;
    private Transform grabPoint;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isGrabbed;
    private bool isReturning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPointTransform)
    {
        grabPoint = grabPointTransform;
        cameraTransform = Camera.main.transform;

        // Save where the object was when picked up
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        isGrabbed = true;
        isReturning = false;
    }

    public void Release()
    {
        isGrabbed = false;
        isReturning = true;

        grabPoint = null;
    }

    private void FixedUpdate()
    {
        // Move toward grab point
        if (isGrabbed && grabPoint != null)
        {
            Vector3 newPos = Vector3.Lerp(rb.position, grabPoint.position, Time.fixedDeltaTime * lerpSpeed);
            rb.MovePosition(newPos);

            float x = 0f;
            float y = 0f;

            if (Input.GetKey(KeyCode.W)) x += 1f;
            if (Input.GetKey(KeyCode.S)) x -= 1f;
            if (Input.GetKey(KeyCode.A)) y += 1f;
            if (Input.GetKey(KeyCode.D)) y -= 1f;

            // Axis definitions
            Vector3 rightAxis = cameraTransform.right;  // tilt relative to camera
            Vector3 upAxis = Vector3.up;                // spin around global up

            Quaternion rotX = Quaternion.AngleAxis(
                x * rotationSpeed * Time.fixedDeltaTime,
                rightAxis);

            Quaternion rotY = Quaternion.AngleAxis(
                y * rotationSpeed * Time.fixedDeltaTime,
                upAxis);

            // Apply in correct order
            rb.MoveRotation(rotY * rotX * rb.rotation);
        }
        // Move back to original position
        else if (isReturning)
        {
            Vector3 newPos = Vector3.Lerp(rb.position, originalPosition, Time.fixedDeltaTime * lerpSpeed);
            rb.MovePosition(newPos);

            Quaternion newRot = Quaternion.Slerp(rb.rotation, originalRotation, Time.fixedDeltaTime * slerpSpeed);
            rb.MoveRotation(newRot);

            bool posDone = Vector3.Distance(rb.position, originalPosition) < 0.05f;
            bool rotDone = Quaternion.Angle(rb.rotation, originalRotation) < 1f;

            // Stop returning when close enough
            if (posDone && rotDone)
            {
                isReturning = false;
            }
        }
    }
}
