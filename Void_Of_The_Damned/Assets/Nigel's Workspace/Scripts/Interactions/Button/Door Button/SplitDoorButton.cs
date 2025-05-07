using UnityEngine;

public class SplitDoorButton : InteractableButton
{
    [Header("Split Door Reference")]
    [SerializeField] private SplitDoorController splitDoor;
    public AudioSource clickSource;
    public AudioClip clickClip;

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

        if (clickSource != null && clickClip != null)
        {
            clickSource.PlayOneShot(clickClip);
        }
    }
}
