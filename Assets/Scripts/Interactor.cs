using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask pickupLayerMask;

    private ObjectGrabbable grabbedObject;
    private InputManager inputManager;

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
            if (hit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                grabbedObject = grabbable;
                grabbedObject.Grab(objectGrabPointTransform);

                // Freeze player + camera
                inputManager.SetMovement(false);
                inputManager.SetLook(false);
            }
        }
    }

    private void ReleaseObject()
    {
        grabbedObject.Release();
        grabbedObject = null;

        // Re-enable player + camera
        inputManager.SetMovement(true);
        inputManager.SetLook(true);
    }
}