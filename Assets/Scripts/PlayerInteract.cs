using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;  
    [SerializeField] private LayerMask mask; 
    private PlayerUI playerUI; 
    //Start is called before the first frame update
    void Start()
    {   
        playerCamera = GetComponentInChildren<Camera>();
        playerUI = GetComponent<PlayerUI>();
    }
    //Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);
        //create a ray from the center of the camera, shooting outwards.
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        RaycastHit hitInfo; //stores information about what was hit by the ray
        if (Physics.Raycast(ray, out hitInfo, interactDistance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                playerUI.UpdateText(hitInfo.collider.GetComponent<Interactable>().promptmessage);
            }
        }
    }
}
