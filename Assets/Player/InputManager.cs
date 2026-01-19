using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static Vector2 Look;

    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;

    public static bool SprintIsHeld;

    public static bool CrouchIsHeld;

    public static bool camLockOn;
    public static event Action<bool> camToggle;

    private static InputAction _moveAction;
    private static InputAction _lookAction;

    private static InputAction _jumpAction;
    private static InputAction _sprintAction;

    private static InputAction _lockOnAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _lookAction = PlayerInput.actions["Look"];

        _jumpAction = PlayerInput.actions["Jump"];
        _sprintAction = PlayerInput.actions["Sprint"];

        _lockOnAction = PlayerInput.actions["LockOn"];
    }

    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        Look = _lookAction.ReadValue<Vector2>();
        Look.y = Mathf.Clamp(Look.y, -90f, 90f);

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        SprintIsHeld = _sprintAction.IsPressed();

        if (_lockOnAction.WasPressedThisFrame())
        {
            camLockOn = !camLockOn;
            camToggle?.Invoke(camLockOn);
        }
    }
}
