using UnityEngine;

public class Potion : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Override the Interact method from the Interactable base class
    protected override void Interact()
    {
        Debug.Log("Player has interacted with the " + gameObject.name);
    }
}
