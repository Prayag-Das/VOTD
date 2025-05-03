using UnityEngine;

public class SplitDoorButton : InteractableButton
{
    [Header("Split Door Reference")]
    [SerializeField] private SplitDoorController splitDoor;

    public override void Interact()
    {
        base.Interact();
        if (splitDoor != null)
        {
            splitDoor.ActivateDoor();
        }
        else
        {
            Debug.LogWarning("SplitDoorButton: No SplitDoorController assigned!");
        }
    }
}
