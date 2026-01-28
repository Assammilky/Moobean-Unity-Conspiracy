using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 10f;

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

        // Save where the object was when picked up
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        isGrabbed = true;
        isReturning = false;

        rb.useGravity = false;
    }

    public void Release()
    {
        isGrabbed = false;
        isReturning = true;

        grabPoint = null;

        rb.useGravity = false;          // keep gravity off while returning
    }

    private void FixedUpdate()
    {
        // Move toward grab point
        if (isGrabbed && grabPoint != null)
        {
            Vector3 newPos = Vector3.Lerp(
                rb.position,
                grabPoint.position,
                Time.fixedDeltaTime * lerpSpeed);

            rb.MovePosition(newPos);
        }
        // Move back to original position
        else if (isReturning)
        {
            Vector3 newPos = Vector3.Lerp(
                rb.position,
                originalPosition,
                Time.fixedDeltaTime * lerpSpeed);

            rb.MovePosition(newPos);

            // Stop returning when close enough
            if (Vector3.Distance(rb.position, originalPosition) < 0.05f)
            {
                isReturning = false;
                rb.useGravity = true; // re-enable physics once it's back
            }
        }
    }
}
