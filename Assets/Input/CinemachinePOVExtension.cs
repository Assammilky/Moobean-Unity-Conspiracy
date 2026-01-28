using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] private float clampAngle = 80f;
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;

    private InputManager inputManager;
    private Vector3 startingRotation;

    protected override void Awake()
    {
        base.Awake();
        startingRotation = transform.localRotation.eulerAngles;
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found! Make sure it exists in the scene.");
        }
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (inputManager == null) return; // prevent errors if not yet assigned

        if (vcam.Follow && stage == CinemachineCore.Stage.Aim)
        {
            Vector2 deltaInput = inputManager.GetLookInput();

            startingRotation.x += deltaInput.x * horizontalSpeed * deltaTime;
            startingRotation.y += deltaInput.y * verticalSpeed * deltaTime;
            startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);

            state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
        }
    }
}
