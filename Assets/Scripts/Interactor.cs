using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IGrabbable
{
    void Grab(Transform grabPoint);
    void Release();
}


public class Interactor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;

    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask pickupLayerMask;

    private InputManager inputManager;

    // Current grabbed object (can be any type that implements IGrabbable)
    private IGrabbable grabbedObject;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabbedObject == null)
            {
                TryGrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }
    }

    private void TryGrabObject()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, interactionRange, pickupLayerMask))
        {
            // Try to find any grabbable object (using interface for flexibility)
            if (hit.transform.TryGetComponent(out IGrabbable grabbable))
            {
                grabbedObject = grabbable;
                grabbedObject.Grab(objectGrabPointTransform);

                inputManager.SetMovement(false);
                inputManager.SetLook(false);
            }
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObject == null) return;

        grabbedObject.Release();
        grabbedObject = null;

        inputManager.SetMovement(true);
        inputManager.SetLook(true);
    }
}
