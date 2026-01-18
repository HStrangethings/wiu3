using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public PlayerInput PlayerInput;

    public Vector2 Movement;
    public Vector2 Look;

    public bool JumpWasPressed;
    public bool JumpIsHeld;
    public bool JumpWasReleased;

    public bool SprintIsHeld;

    public bool CrouchIsHeld;

    public bool camLockOn;
    public event Action<bool> camToggle;

    private InputAction _moveAction;
    private InputAction _lookAction;

    private InputAction _jumpAction;
    private InputAction _sprintAction;

    private InputAction _lockOnAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

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
