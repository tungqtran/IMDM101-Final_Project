using UnityEngine;

/// <summary>
/// Efficient first-person player controller with movement, jumping, crouching, and mouse look.
/// 
/// SETUP INSTRUCTIONS:
/// 1. Create a GameObject called "Player" with a CharacterController component
/// 2. Create a child object "CameraHolder" and attach a Camera to it
/// 3. Assign the Player GameObject this script
/// 4. In the Inspector, assign the CameraHolder's Transform to the "Camera Transform" field
/// 5. Adjust the Inspector parameters to tune movement feel
/// 
/// CONTROLS:
/// - W/A/S/D: Move forward/left/backward/right
/// - Space: Jump
/// - Left Shift: Crouch (hold to maintain)
/// - Mouse: Look around
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    [SerializeField] private Transform cameraTransform;
    private CharacterController characterController;

    [Header("=== MOVEMENT SETTINGS ===")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float groundDrag = 5f;

    [Header("=== JUMP SETTINGS ===")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag_Jump = 3f;

    [Header("=== CROUCH SETTINGS ===")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    [Header("=== GRAVITY & PHYSICS ===")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDrag_Falling = 10f;

    [Header("=== MOUSE SETTINGS ===")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;

    [Header("=== GROUND DETECTION ===")]
    [SerializeField] private float groundDragDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // State variables
    private Vector3 velocity = Vector3.zero;
    private float xRotation = 0f;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private float currentHeight;
    private Vector3 desiredVelocity = Vector3.zero;
    private float desiredDrag = 5f;

    void Start()
    {
        // Get required components
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("PlayerController: CharacterController component not found! Add one to this GameObject.");
            return;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("PlayerController: Camera Transform not assigned! Assign the camera holder in the Inspector.");
            return;
        }

        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHeight = characterController.height;
    }

    void Update()
    {
        // Ground check using raycast
        CheckGrounded();

        // Handle mouse look (always active)
        HandleMouseLook();

        // Handle movement input
        HandleMovementInput();

        // Handle crouch
        HandleCrouch();

        // Apply velocity and gravity
        ApplyGravity();
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Checks if the player is touching the ground using a raycast.
    /// </summary>
    private void CheckGrounded()
    {
        // Raycast downward from center of character controller
        Ray groundCheck = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(groundCheck, characterController.height / 2 + groundDragDistance, groundLayer);
    }

    /// <summary>
    /// Handles WASD movement input and sprinting.
    /// </summary>
    private void HandleMovementInput()
    {
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Determine current speed
        float currentSpeed = walkSpeed;
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = sprintSpeed;

        // Calculate movement direction (relative to player)
        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        desiredVelocity = moveDirection.normalized * currentSpeed;

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            Jump();
        }

        // Apply movement
        velocity.x = Mathf.Lerp(velocity.x, desiredVelocity.x, Time.deltaTime * groundDrag);
        velocity.z = Mathf.Lerp(velocity.z, desiredVelocity.z, Time.deltaTime * groundDrag);
    }

    /// <summary>
    /// Handles mouse input for first-person look.
    /// </summary>
    private void HandleMouseLook()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down (with limits)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    /// <summary>
    /// Handles crouch input and height transitions.
    /// </summary>
    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching)
        {
            isCrouching = true;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && isCrouching)
        {
            // Check if there's space to stand up
            if (CanStandUp())
            {
                isCrouching = false;
            }
        }

        // Smoothly transition height
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        characterController.height = currentHeight;

        // Adjust center to keep character grounded
        Vector3 center = characterController.center;
        center.y = currentHeight / 2f;
        characterController.center = center;
    }

    /// <summary>
    /// Checks if the player has enough space to stand up.
    /// </summary>
    private bool CanStandUp()
    {
        // Raycast upward to check for obstacles
        return !Physics.Raycast(transform.position + Vector3.up * (crouchHeight / 2f), Vector3.up, (standHeight - crouchHeight) / 2f, groundLayer);
    }

    /// <summary>
    /// Applies gravity to the player.
    /// </summary>
    private void ApplyGravity()
    {
        if (isGrounded)
        {
            // Small downward force to keep grounded
            if (velocity.y < 0)
                velocity.y = -1f;
        }
        else
        {
            // Apply gravity when in air
            velocity.y += gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    private void Jump()
    {
        // Calculate jump velocity using physics formula: v = sqrt(2 * g * h)
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    /// <summary>
    /// Unlock and show cursor (call when entering menu, etc.)
    /// </summary>
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Lock and hide cursor (call when resuming gameplay).
    /// </summary>
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

