using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    private PlayerInput playerInput;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            playerInput = new PlayerInput();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public Vector2 GetMovementInput()
    {
        return playerInput.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return playerInput.Player.Look.ReadValue<Vector2>();
    }

}
