using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerInput : MonoBehaviour
{
    private playerController _playerController;

    [Header("Keybinds")]
    KeyCode jumpKey = KeyCode.Space;
    KeyCode sprintKey = KeyCode.LeftShift;

    public Vector3 movement;

    [Header("Mouse")]
    public Vector2 mouse;

    public float sensX, sensY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerController = GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = Vector3.zero;

        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");

        mouse.x += Input.GetAxis("Mouse X") * sensX;
        mouse.y -= Input.GetAxis("Mouse Y") * sensY;
        mouse.y = Mathf.Clamp(mouse.y, -90f, 90f);

        if (Input.GetKey(sprintKey))
        {
            _playerController.changeState(playerController.states.SPRINT);
        }
        else if (movement.magnitude != 0)
        {
            _playerController.changeState((playerController.states.WALK));
        }

        if (Input.GetKey(jumpKey))
        {
            if (_playerController.onGround)
            {
                _playerController.changeState((playerController.states.JUMP));
            }
        }
    }
}

