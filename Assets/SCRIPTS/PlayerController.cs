using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 9f;
    public float groundDrag = 5f;
    public float airMultiplier = 0.5f;

    [Header("Jumping")]
    public float jumpForce = 7f;
    public float jumpCoolDown = 0.25f;
    private bool readyToJump = true;

    [Header("Double Jump - CARD")]
    public bool doubleJumpUnlocked = false;
    private bool canDoubleJump = false;
    private float doubleJumpTimer;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Speed Boost - CARD")]
    public float normalSpeed;
    private float originalMoveSpeed;
    private bool speedBoostActive = false;
    private float speedBoostTimer;

    [Header("References")]
    public Transform orientation;
    public Joystick mobileJoystick;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private float baseJumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
        controls.Player.Jump.performed += ctx => AttemptJump();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        rb.freezeRotation = true;
        baseJumpForce = jumpForce;
        originalMoveSpeed = moveSpeed;
        normalSpeed = moveSpeed;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), grounded ? Color.green : Color.red);

        GetInput();
        SpeedControl();
        HandleCardTimers();
                
        bool isMoving = moveInput.magnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.groundedRightNow = grounded; 
            AudioManager.instance.TryPlayFootstep(isMoving, isRunning);
        }

        if (grounded)
        {
            rb.linearDamping = groundDrag;
            canDoubleJump = doubleJumpUnlocked;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        Vector2 keyboardInput = controls.Player.Move.ReadValue<Vector2>();
        Vector2 joystickInput = (mobileJoystick != null) ? mobileJoystick.axis : Vector2.zero;
        moveInput = keyboardInput + joystickInput;

        if (moveInput.magnitude < 0.05f) moveInput = Vector2.zero;
    }

    private void MovePlayer()
    {
        if (moveInput == Vector2.zero) return;

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Acceleration);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Acceleration);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (moveInput == Vector2.zero && grounded)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0, rb.linearVelocity.y, 0), Time.deltaTime * 20f);
            if (flatVel.magnitude < 0.1f)
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public void AttemptJump()
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
        else if (doubleJumpUnlocked && canDoubleJump && !grounded)
        {
            canDoubleJump = false;
            Jump();
            if (doubleJumpUnlocked)
            {
                Invoke(nameof(ResetDoubleJump), 0.1f);
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() => readyToJump = true;

    private void ResetDoubleJump() => canDoubleJump = true;

    private void HandleCardTimers()
    {
        if (doubleJumpUnlocked)
        {
            doubleJumpTimer -= Time.deltaTime;
            if (doubleJumpTimer <= 0)
            {
                doubleJumpUnlocked = false;
                canDoubleJump = false;
                Debug.Log("Double Jump CARD expired");
                                
                if (AudioManager.instance != null)
                    AudioManager.instance.PlayPowerUpEnd();
               
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = Color.white;
            }
        }

        if (speedBoostActive)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0)
            {
                speedBoostActive = false;
                moveSpeed = originalMoveSpeed;
                Debug.Log("Speed Boost CARD expired");
                               
                if (AudioManager.instance != null)
                    AudioManager.instance.PlayPowerUpEnd();
                
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = Color.white;
            }
        }
    }

    public void ActivateDoubleJump(float duration)
    {
        doubleJumpUnlocked = true;
        canDoubleJump = true;
        doubleJumpTimer = duration;
        Debug.Log("Double Jump CARD activated!");

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.cyan;
    }

    public void StartSpeedBuff(float speedMultiplier, float duration)
    {
        speedBoostActive = true;
        moveSpeed = originalMoveSpeed * speedMultiplier;
        speedBoostTimer = duration;
        Debug.Log($"Speed Boost CARD activated! Speed: {moveSpeed}");

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.yellow;
    }
}

//Este es el controlador de jugador, tambien tiene conexion con los buffs de las cartas para que se muevan Y! Tambien cambien el color del player para que sea mas evidente que esta en un estado alterado