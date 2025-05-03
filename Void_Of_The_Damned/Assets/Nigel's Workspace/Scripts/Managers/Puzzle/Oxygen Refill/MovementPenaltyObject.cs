using UnityEngine;
using System.Collections;


public class MovementPenaltyObject : MonoBehaviour
{
    [Header("Penalty Settings")]
    [SerializeField] private float penaltyMoveSpeed = 2f;
    

    private PlayerController player;
    private float originalMoveSpeed;
    private float originalSprintSpeed;
    private bool isBeingHeld = false;

    public void OnPickup(PlayerController playerController)
    {
        if (isBeingHeld) return;

        player = playerController;
        originalMoveSpeed = player.GetMoveSpeed();
        originalSprintSpeed = player.GetSprintSpeed();

        player.SetMoveSpeed(penaltyMoveSpeed);
        player.SetSprintAllowed(false); // Disable sprint input

        isBeingHeld = true;
        Debug.Log("Movement penalty applied.");
    }

    public void OnDrop()
    {
        if (!isBeingHeld || player == null) return;

        player.SetMoveSpeed(originalMoveSpeed);
        player.SetSprintAllowed(true); // Re-enable sprint

        Debug.Log("Movement penalty removed.");
        isBeingHeld = false;
        player = null;
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.SetMoveSpeed(originalMoveSpeed);
            player.SetSprintSpeed(originalSprintSpeed);
            player.SetMovementEnabled(true);
        }
    }
}