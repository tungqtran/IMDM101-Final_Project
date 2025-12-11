using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;  
    [SerializeField] private LayerMask mask; 
    Interactable interactable;

    //Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }
    //Update is called once per frame
    void Update()
    {
        //create a ray from the center of the camera, shooting outwards.
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance, mask))
        {
            //Check if the object we hit has an Interactable component
            interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                //If the player presses the E key, interact with the object
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
    }
}
