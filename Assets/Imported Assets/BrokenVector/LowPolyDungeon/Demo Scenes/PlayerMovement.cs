using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float baseSpeed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float sprintSpeed = 5f;

    public float standingHeight = 2.0f;
    public float crouchHeight = 1.0f;
    public float walkSpeed = 5.0f;
    public float crouchSpeed = 2.5f;

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

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Input.GetButton("Fire3"))
            speedBoost = sprintSpeed;
        else
            speedBoost = 1f;


        Vector3 move = transform.right * x + transform.forward * z;

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
