using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance => instance;

    private PlayerInput playerInput;

    // Control flags
    public bool CanMove { get; private set; } = true;
    public bool CanLook { get; private set; } = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        playerInput = new PlayerInput();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // These are what your player & camera use
    public Vector2 GetMovementInput()
    {
        if (!CanMove) return Vector2.zero;
        return playerInput.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        if (!CanLook) return Vector2.zero;
        return playerInput.Player.Look.ReadValue<Vector2>();
    }

    // These are what your Interactor uses
    public void SetMovement(bool state)
    {
        CanMove = state;
    }

    public void SetLook(bool state)
    {
        CanLook = state;
    }
}
