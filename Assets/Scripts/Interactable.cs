using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    //Message displayed when the player looks at the object
    public string promptmessage;
    
    public void baseInteract()
    {
        Interact();
    }
    //Method to be overridden by subclasses
    public void Interact()
    {
        //no implementation here
    }

}
