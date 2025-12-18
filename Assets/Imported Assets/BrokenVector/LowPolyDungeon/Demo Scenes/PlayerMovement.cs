using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public CharacterController controller;
    [Header("Movement Settings")]
    [SerializeField] public float baseSpeed = 12f;
    [SerializeField] public float walkSpeed = 5.0f;
    
    [SerializeField] public float sprintSpeed = 5f;

    [Header("Physics Settings")]
    [SerializeField] public float gravity = -9.81f;
    [SerializeField] public float jumpHeight = 3f;


    [Header("Crouch Settings")]
    [SerializeField] public float standingHeight = 2.0f;
    [SerializeField] public float crouchHeight = 1.0f;
    
    [SerializeField] public float crouchSpeed = 2.5f;

    float speedBoost = 1f;
    Vector3 velocity;
    private bool isCrouching = false;
    private float originalCameraYPos;
    public Transform playerCamera; // Drag your main camera here in the Inspector

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera != null)
        {
            // Store the camera's original relative Y position
            originalCameraYPos = playerCamera.localPosition.y;
        }
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) // Use LeftControl to toggle crouch
        {
            ToggleCrouch();
        }

        float x = Input.GetAxis("Horizontal"); // Getting A/D values or Left/Right arrows values bewteen -1 and 1
        float z = Input.GetAxis("Vertical"); // Getting W/S values or Up/Down arrows values between -1 and 1

        if (Input.GetButton("Fire3"))
            speedBoost = sprintSpeed;
        else
            speedBoost = 1f;

        /* Code below does vector addtion/multiplication. Hence why it looks like it does make sense when it does.
        transform.right and transform.forward are vectors. 
        vectors has vector addition/multiplication properties 
        1. Vector addition: Vector + Vector = Vector
        2. Vector multiplication: Vector * scalar = Vector
        So when we do transform.right * x, we are multiplying the vector transform.right with the scalar x to get a new vector.
        Similarly, transform.forward * z gives us another vector.
        Finally, we add the two vectors together to get the final movement vector 'move'.
        */
        Vector3 move = transform.right * x + transform.forward * z; 
        /* The final move vector is then passed to controller.Move() function which moves the character controller in the direction of the move vector.
        Note that we multiply the move vector with (baseSpeed + speedBoost) to scale the movement speed.
        We also multiply with Time.deltaTime to make the movement frame rate independent.
        */
        controller.Move(move * (baseSpeed + speedBoost) * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            float previousHeight = controller.height;

            controller.height = crouchHeight;
            controller.center = new Vector3(
                controller.center.x,
                controller.center.y - (previousHeight - crouchHeight) / 2f,
                controller.center.z
            );

            if (playerCamera != null)
            {
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    originalCameraYPos - (standingHeight - crouchHeight) / 2f,
                    playerCamera.localPosition.z
                );
            }
        }
        else
        {
            // Overhead check so you don't stand up into a ceiling
            if (Physics.Raycast(transform.position, Vector3.up, standingHeight))
            {
                // blocked, don't stand
                return;
            }

            float previousHeight = controller.height;

            controller.height = standingHeight;
            controller.center = new Vector3(
                controller.center.x,
                controller.center.y + (standingHeight - previousHeight) / 2f,
                controller.center.z
            );

            if (playerCamera != null)
            {
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    originalCameraYPos,
                    playerCamera.localPosition.z
                );
            }
        }
    }

}
