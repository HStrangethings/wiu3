using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    private Rigidbody _rb;
    private playerInput _playerInput;
    public PlayerData _playerData;

    public float _camDistOffset;
    public float _camHeightOffset;


    public float vel;
    private float _maxSpeed;
    private float _speed;

    public states currentState;

    public bool onGround;
    public enum states
    {
        IDLE = 0,
        WALK,
        SPRINT,
        JUMP,
        LAUNCHED,
        FALL,
        TOTAL_STATES
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<playerInput>();


        _rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case states.IDLE:
                _rb.linearDamping = 3.0f;
                break;
            case states.FALL:
                if (onGround)
                {
                    changeState(states.IDLE);
                }
                break;
            case states.JUMP:
                jump();
                onGround = false;
                break;
            case states.WALK:
                _maxSpeed = _playerData.getMaxSpeed();
                _speed = _playerData.getSpeed();
                break;
            case states.SPRINT:
                _maxSpeed = _playerData.getMaxSpeed() * _playerData.getSprintMul();
                _speed = _playerData.getSpeed() * _playerData.getSprintMul();
                break;

        }

        if (currentState != states.IDLE)
        {
            _rb.linearDamping = 0f;
        }

        transform.rotation = Quaternion.Euler(0, _playerInput.mouse.x, 0);
        Camera.main.transform.rotation = Quaternion.Euler(_playerInput.mouse.y, _playerInput.mouse.x, 0);
        Camera.main.transform.position = transform.position - Camera.main.transform.forward * _camDistOffset + Vector3.up * _camHeightOffset;

        vel = _rb.linearVelocity.magnitude;
        if(vel < 0.1)
        {
            changeState(states.IDLE);
        }
        if(vel > _maxSpeed)
        {
            _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, _maxSpeed);
        }
        
        Mathf.Clamp(_rb.linearVelocity.z, -_playerData.getMaxSpeed(), _playerData.getMaxSpeed());
        Mathf.Clamp(_rb.linearVelocity.x, -_playerData.getMaxSpeed(), _playerData.getMaxSpeed());

        if (_rb.linearVelocity.y < 0)
        {
            changeState(states.FALL);
        }
        else if (_rb.linearVelocity.y > 0)
        {
            changeState(states.LAUNCHED);
        }
    }

    private void FixedUpdate()
    {
        Vector3 playerDir = _playerInput.movement.z * transform.forward + _playerInput.movement.x * transform.right;
        _rb.AddForce(playerDir * _speed, ForceMode.Force);

    }
    private void jump()
    {
        Vector3 jumpForce = transform.up * _playerData.getJumpHeight();
        _rb.AddForce(jumpForce, ForceMode.Impulse);
    }
    public void changeState(states state)
    {

        currentState = state;
    }
}
