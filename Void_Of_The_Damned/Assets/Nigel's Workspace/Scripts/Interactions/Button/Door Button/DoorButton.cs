using UnityEngine;

public class DoorButton : InteractableButton
{
    [SerializeField] private DoorController door;

    public override void Interact()
    {
        base.Interact();
        if (door != null)
        {
            door.ActivateDoor();
        }
    }
}
