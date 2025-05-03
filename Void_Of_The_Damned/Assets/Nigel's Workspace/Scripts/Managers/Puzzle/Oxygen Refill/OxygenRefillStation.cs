using UnityEngine;
using System.Collections;

public class OxygenRefillStation : MonoBehaviour, IInteractable
{
    [Header("Task Requirements")]
    [SerializeField] private PuzzleElement task1Requirement;
    [SerializeField] private PuzzleElement currentTaskElement;
    [SerializeField] private PuzzleRoomManager roomManager;

    [Header("Station Models")]
    [SerializeField] private GameObject openModel;
    [SerializeField] private GameObject closedModel;

    [Header("Settings")]
    

    private bool taskCompleted = false;

    private void Start()
    {
        // Make sure door starts open if task not done
        openModel.SetActive(true);
        closedModel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (taskCompleted) return;

        OxygenTankIdentifier tank = other.GetComponent<OxygenTankIdentifier>();
        if (tank != null)
        {
            if (task1Requirement != null && !task1Requirement.IsCompleted)
            {
                Debug.Log("Cannot refill tank. Oxygen leak must be sealed first.");
                return;
            }

            CompleteRefillTask();

            MovementPenaltyObject penalty = tank.GetComponent<MovementPenaltyObject>();
            if (penalty != null)
            {
                penalty.OnDrop(); // Ensure cleanup before destruction
            }


            Destroy(tank.gameObject);
        }
    }

    private void CompleteRefillTask()
    {
        Debug.Log("Oxygen successfully refilled!");

        taskCompleted = true;

        currentTaskElement.MarkCompleted();
        roomManager.CheckPuzzleProgress();

        if (openModel) openModel.SetActive(false);
        if (closedModel)
        {
            Debug.Log("Activating Closed Model: " + closedModel.name);
            closedModel.SetActive(true);
        }
    }

    public void Interact()
    {
        if (taskCompleted)
        {
            Debug.Log("Oxygen refill complete.");
        }
        else
        {
            Debug.Log("Bring the oxygen tank here to refill.");
        }
    }
}
