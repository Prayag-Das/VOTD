using UnityEngine;

public class DoorButton : InteractableButton
{
    [SerializeField] private DoorController door;
    public AudioSource clickSource;
    public AudioClip clickClip;

    public override void Interact()
    {
        base.Interact();

        if (door != null)
        {
            door.ActivateDoor();
        }

        if (clickSource != null && clickClip != null)
        {
            clickSource.PlayOneShot(clickClip);
        }
    }
}
