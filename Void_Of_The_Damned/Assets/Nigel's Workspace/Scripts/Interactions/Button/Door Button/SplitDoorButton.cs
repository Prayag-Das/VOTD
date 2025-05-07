using UnityEngine;

public class SplitDoorButton : InteractableButton
{
    [Header("Split Door Reference")]
    [SerializeField] private SplitDoorController splitDoor;

    // If this button also has a DoorUnlock component, we'll gate on it.
    private DoorUnlock unlockLogic;

    private void Awake()
    {
        unlockLogic = GetComponent<DoorUnlock>();
    }

    public override void Interact()
    {
        // If there is unlockLogic and it is not yet unlocked, block interaction
        if (unlockLogic != null && !unlockLogic.IsUnlocked)
        {
            // Optional: play a “locked” SFX or flash your statusLockText red here
            return;
        }

        // Otherwise proceed as normal
        base.Interact();

        if (splitDoor != null)
            splitDoor.ActivateDoor();
    }
}