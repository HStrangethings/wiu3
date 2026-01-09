using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public PlayerData playerData;

    #region FSM
    //Simple FSM

    enum STATE
    {
        IDLE = 0,
        WALK,
        SPRINT,
        CROUCH,
        SLIDE,
        ADS,
        INAIR,
        FALL,
        LAUNCH,
        GLIDE,
        GROUNDED
    }

    private STATE currentXState;
    private STATE currentYState;
    #endregion

    #region Movement Variables
    public bool sliding;

    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    public float mouseSmoothTime = 0.03f; // smoothing duration in seconds
    public float mouseSensitivity = 1.0f;

    //overall stuff
    public float Gravity = 9.8f;
    public float Friction = 1.0f;
    public float movementSens = 0.1f;

    private float speed;
    private float maxSpeed;

    private float sprintSpeedMul;
    private float crouchSpeedMul;

    private float InitialJumpVelocity;
    private float InitialSlideVelocity;

    public float MaxJump = 1;
    private float _jumpCount;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    //movement
    public float HorizontalVelocity;
    public float VerticalVelocity;
    public Vector3 moveDirection;
    public bool isGrounded = false;
    public LayerMask groundLayer;
    public float groundRayLength;

    //camera 
    private float originalCamY;
    private float timer;
    public float bobbingOffset;
    public float bobbingFreq;
    public float bobbingAmp;

    //Recoil
    public float camRotationX = 0f;
    public float camDeltaRotationX = 0f;

    // Use for change FOV
    public float sprintFOVMultiplier = 1.5f; // multiplier for sprinting
    public float slideFOVMultiplier = 2f; // multiplier for sliding
    private float sprintFOV; // stores FOV value when sprinting
    private float slideFOV; // stores FOV value when sliding
    private float normalFOV; // stores FOV value when walking
    private float ADSFOV;
    private float currentFOV; // stores current FOV

    #endregion

    #region Weapons

    [Header("Weapon")]

    // Use for camera shake
    public float shakeDuration = 0.5f; // duration of the shake
    public float shakeMagnitude = 0.2f; // strength of the shake
    public float shakeTimeRemaining = 0.0f; // time remaining since shake starts
    private Vector3 originalPosition; // stores original position to return to
    private Vector3 shakeOffset; 

    // For weapon switching
    private int currentWeaponIndex;

    [Header("Pick Up")]
    [SerializeField] private float pickUpRange;
    [SerializeField] private LayerMask itemLayer;

    [SerializeField] private float throwForce;
    #endregion

    #region UI
    //UI
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] TextMeshProUGUI VelocityText;
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentXState = STATE.IDLE;
        currentYState = STATE.INAIR;

        rb = GetComponent<Rigidbody>();

        //for camera bobbing
        originalCamY = Camera.main.transform.localPosition.y;
        timer = 0f;
        bobbingOffset = 0f;
        camRotationX = Camera.main.transform.localEulerAngles.x;
        camDeltaRotationX = 0;

        // for camera shake
        originalPosition = Camera.main.transform.localPosition;

        // for change of FOV
        currentFOV = normalFOV = Camera.main.fieldOfView;
        sprintFOV = normalFOV * sprintFOVMultiplier;
        slideFOV = normalFOV * slideFOVMultiplier;
        ADSFOV = normalFOV * 0.5f;

        // Initialize movement variables that depend on playerData
        speed = playerData.getSpeed();
        maxSpeed = playerData.getMaxSpeed();
        sprintSpeedMul = playerData.getSprintMul();
        crouchSpeedMul = playerData.getCrouchMul();
        InitialJumpVelocity = playerData.getJumpVelocity();
        InitialSlideVelocity = playerData.getSlideVelocity();
    }

    #region Updates
    private void Update()
    {
        NextStateCheck();
        //stateText.text = "XState : " + currentXState.ToString() + "\n YState : " + currentYState.ToString();

        SizeUpdate();

        MovementUpdate();
        //VelocityText.text = "Horizontal Velocity : " + HorizontalVelocity + "\n Vertical Velocity : " + VerticalVelocity;
    }
    private void LateUpdate()
    {
        Look();

        CameraEffects();
    }
    private void SizeUpdate()
    {
        //if (currentXState == STATE.CROUCH)
        //    characterController.height = 1; //Mathf.Lerp(1, 3, Time.deltaTime * 0.01f);
        //else
        //    characterController.height = 2f; //Mathf.Lerp(3, 1, Time.deltaTime * 0.01f);
    }
    private void MovementUpdate()
    {
        if (currentXState == STATE.WALK)
            Walk();

        if (currentXState == STATE.SPRINT)
            Sprint();

        if (currentXState == STATE.CROUCH)
            Crouch();

        if (currentXState == STATE.ADS)
            ADS();

        if ((InputManager.JumpWasPressed ||InputManager.JumpIsHeld)&& currentYState == STATE.GROUNDED)
            Jump();

        if (currentXState == STATE.SLIDE && !sliding)
            Slide();

        ExternalForces();
        Move();
    }
    #endregion

    #region Camera
    private void Look()
    {
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, InputManager.Look, ref currentMouseDeltaVelocity, mouseSmoothTime);

        yRotation += currentMouseDelta.x * mouseSensitivity;
        xRotation -= currentMouseDelta.y * mouseSensitivity;

        xRotation = Mathf.Clamp(xRotation, -70, 70);

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);

    }
    private void CameraEffects()
    {
        CameraBob();

        CameraShake();

        CameraRecoil();

        Camera.main.transform.localPosition = originalPosition + shakeOffset + new Vector3(0, bobbingOffset, 0);

        CameraFOV();

    }
    private void CameraBob()
    {
        if (isGrounded) return;

        Transform cameraTransform = Camera.main.transform;

        bool isMoving = Mathf.Abs(HorizontalVelocity) > 0;

        if (isMoving)
        {
            // Increment the bobbing timer as player is moving
            timer += Time.deltaTime * bobbingFreq * Mathf.Abs(HorizontalVelocity);
            // Calculate the new Y position for camera bobbing using a sine wave
            bobbingOffset = Mathf.Sin(timer) * bobbingAmp;
        }
        else
        {
            bobbingOffset = Mathf.Lerp(bobbingOffset, 0, Time.deltaTime);
        }
        // Add the originalCameraY with the bobbingOffset value as new Y position value
        //cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, originalCamY + bobbingOffset, cameraTransform.localPosition.z);
    }
    private void CameraShake()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            // Ensure we don't accumulate small positional errors over time
            shakeOffset = Vector3.zero;
        }
    }
    private void CameraFOV()
    {
        if (currentXState == STATE.SPRINT)
        {
            currentFOV = Mathf.Lerp(currentFOV, sprintFOV, HorizontalVelocity * Time.deltaTime);
        }
        else if (currentXState == STATE.SLIDE)
        {
            currentFOV = Mathf.Lerp(currentFOV, slideFOV, HorizontalVelocity * Time.deltaTime);
        }
        else if (currentXState == STATE.ADS)
        {
            currentFOV = Mathf.Lerp(currentFOV, ADSFOV, Time.deltaTime * 10);
        }
        else
        {
            currentFOV = Mathf.Lerp(currentFOV, normalFOV, HorizontalVelocity * Time.deltaTime);
        }

            Camera.main.fieldOfView = currentFOV;
    }
    private void CameraRecoil()
    {
        if (shakeTimeRemaining > 0)
        {
            xRotation -= Time.deltaTime;
        }      
    }
    #endregion

    #region Movement
    private void Move()
    {
        moveDirection = InputManager.Movement.x * transform.right + InputManager.Movement.y * transform.forward;

        Mathf.Lerp(HorizontalVelocity, 0, maxSpeed);
        Mathf.Lerp(VerticalVelocity, -maxSpeed, maxSpeed);

        if (HorizontalVelocity < movementSens)
            HorizontalVelocity = 0;

        Vector3 hMove = moveDirection * HorizontalVelocity;
        Vector3 vMove = transform.up * VerticalVelocity;
       
        Vector3 Move = hMove + vMove;
        rb.AddForce(Move);
    }
    private void Walk()
    {
        HorizontalVelocity += speed;
    }
    private void Sprint()
    {
        HorizontalVelocity += speed * sprintSpeedMul;
    }
    private void Crouch()
    {
        HorizontalVelocity += speed * crouchSpeedMul;
    }
    private void ADS()
    {
        HorizontalVelocity += speed * 0.25f;
    }
    private void Slide()
    {
        HorizontalVelocity = InitialSlideVelocity;
        sliding = true;
    }
    private void Jump()
    {
        VerticalVelocity = InitialJumpVelocity;
        currentYState = STATE.INAIR;
    }
    #endregion

    #region Combat
    private void InteractCheck()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpRange, itemLayer))
        {
           
        }
    }
   
    //delte this section if not needed
    //private void PickUp(Item item, RaycastHit hit)
    //{

    //}
    //private void Interact(Item item, RaycastHit hit)
    //{

    //}
    //private void Drop()
    //{
      
    //}

    #endregion

    private void ExternalForces()
    {
        //grounded check 
        Ray groundRay = new Ray(transform.position, -transform.up);
        isGrounded = Physics.Raycast(groundRay, out RaycastHit groundHit, groundRayLength, groundLayer);

        //friction
        if (currentXState == STATE.SLIDE)
        {
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0, Time.deltaTime);
        }
        else
        {
            HorizontalVelocity -= Friction * HorizontalVelocity * 0.1f;

        }

        //gravity 
        if (currentYState == STATE.FALL || currentYState == STATE.INAIR || currentYState == STATE.LAUNCH)
        {
            VerticalVelocity -= Gravity * 2 * Time.deltaTime;
        }
        else if (isGrounded && VerticalVelocity < 0)
        {
            VerticalVelocity = -1;
        }
    }
    private void NextStateCheck()
    {
        //YFSM
        switch (currentYState)
        {
            case STATE.GROUNDED:

                if(!isGrounded)
                {
                    currentYState = STATE.INAIR;
                    break;
                }
                break;

            case STATE.INAIR:

                if (VerticalVelocity > 0)
                {
                    currentYState = STATE.LAUNCH;
                    break;
                }
                else if (VerticalVelocity < 0)
                {
                    currentYState = STATE.FALL;
                    break;
                }
                break;

            case STATE.LAUNCH:

                if (VerticalVelocity < 0)   
                {
                    currentYState = STATE.FALL;
                    break;
                }
                break;

            case STATE.FALL:

                if (isGrounded)
                {
                    _jumpCount = 0;
                    currentYState = STATE.GROUNDED;
                    break;
                }
                break;
        }

        //XFSM
        switch (currentXState)
        {
            case STATE.IDLE:

                if (InputManager.CrouchIsHeld)
                {
                    currentXState = STATE.CROUCH;
                    break;
                }

                if(InputManager.ADSIsHeld)
                {
                    currentXState = STATE.ADS;
                    break;
                }

                if (Mathf.Abs(InputManager.Movement.magnitude) > movementSens)
                {
                    if (InputManager.SprintIsHeld)
                    {
                        currentXState = STATE.SPRINT;
                        break;
                    }
                    currentXState = STATE.WALK;
                    break;
                }
                break;

            case STATE.WALK:

                if (Mathf.Abs(InputManager.Movement.magnitude) < movementSens)
                {
                    currentXState = STATE.IDLE;
                    break;
                }

                if (InputManager.ADSIsHeld)
                {
                    currentXState = STATE.ADS;
                    break;
                }

                if (InputManager.CrouchIsHeld)
                {
                    currentXState = STATE.CROUCH;
                    break;
                }
                else if (InputManager.SprintIsHeld)
                {
                    currentXState = STATE.SPRINT;
                    break;
                }
                break;

            case STATE.SPRINT:

                if (!InputManager.SprintIsHeld)
                {
                    if (Mathf.Abs(InputManager.Movement.magnitude) < movementSens)
                    {
                        currentXState = STATE.IDLE;
                        break;
                    }
                    else
                    {
                        currentXState = STATE.WALK;
                    }
                }

                if (InputManager.ADSIsHeld)
                {
                    currentXState = STATE.ADS;
                    break;
                }

                if (InputManager.CrouchIsHeld)
                {
                    currentXState = STATE.SLIDE;
                }
                break;

            case STATE.CROUCH:

                if (!InputManager.CrouchIsHeld)
                {
                    if (Mathf.Abs(InputManager.Movement.magnitude) < movementSens)
                    {
                        currentXState = STATE.IDLE;
                        break;
                    }
                    else
                    {
                        currentXState = STATE.WALK;
                    }
                }

                if (InputManager.ADSIsHeld)
                {
                    currentXState = STATE.ADS;
                    break;
                }
                break;

            case STATE.SLIDE:

                if(currentYState == STATE.LAUNCH)
                {
                    currentXState = STATE.IDLE;
                    sliding = false;
                    break;
                }
                if (HorizontalVelocity < 10 && !InputManager.CrouchIsHeld)
                {
                    if (Mathf.Abs(InputManager.Movement.magnitude) < movementSens)
                    {
                        currentXState = STATE.IDLE;
                        sliding = false;
                        break;
                    }
                    else
                    {
                        currentXState = STATE.WALK;
                        sliding = false;
                        break;
                    }
                }
                break;

            case STATE.ADS:

                if(InputManager.ADSWasReleased || !InputManager.ADSIsHeld)
                {
                    if (Mathf.Abs(InputManager.Movement.magnitude) < movementSens)
                    {
                        currentXState = STATE.IDLE;
                        break;
                    }
                    else
                    {
                        currentXState = STATE.WALK;
                        break;
                    }
                }

                break;


        }
    }

}
