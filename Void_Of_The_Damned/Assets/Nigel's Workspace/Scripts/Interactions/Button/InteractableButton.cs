using UnityEngine;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public virtual void Interact()
    {
        Debug.Log("Button was pressed!");

        // TODO: Add sound, animation, or trigger logic here.
        // Example: call GameManager.StartPuzzle();
    }
}
