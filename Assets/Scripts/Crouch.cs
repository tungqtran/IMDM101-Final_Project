using UnityEngine;

public class Crouch : MonoBehaviour
{

    CharacterController characterController;
    public float normalHeight = 2.0f;
    public float crouchHeight = 1.0f;
    private bool isCrouching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null) {
            Debug.LogError("CharacterController not found on the Crouching Script for Player");
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            ToggleCrouch();
        }
    }

    void ToggleCrouch() {
        isCrouching = !isCrouching;

        if (isCrouching) {
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2f, 0);
        } else {
            characterController.height = normalHeight;
            characterController.center = new Vector3(0, normalHeight / 2f, 0);
        }
    }
}
