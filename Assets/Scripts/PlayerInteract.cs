using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;   

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
    }
}
