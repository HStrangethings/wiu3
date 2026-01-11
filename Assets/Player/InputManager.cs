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

    public static bool ADSIsHeld;
    public static bool ADSWasReleased;

    public static bool camLockOn;

    private InputAction _moveAction;
    private InputAction _lookAction;

    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _adsAction;

    private InputAction _lockOnAction;

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

        if (_lockOnAction.WasPressedThisFrame()) { camLockOn = !camLockOn; }
    }
}
