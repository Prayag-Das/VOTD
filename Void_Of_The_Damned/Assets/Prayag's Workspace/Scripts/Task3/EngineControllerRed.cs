using UnityEngine;

public class EngineControllerRed : MonoBehaviour
{
    [Header("Task Dependencies")]
    [SerializeField] private PuzzleRoomManager roomManager;
    [SerializeField] private PuzzleElement currentTaskElement;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject boxPrefab;

    private bool taskCompleted = false;

    private void Start()
    {
        // Initialize from saved state if task already marked complete
        if (currentTaskElement != null && currentTaskElement.IsCompleted)
        {
            taskCompleted = true;
            SpawnBox();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (taskCompleted) return;

        if (other.CompareTag("Task-3-red"))
        {
            if (currentTaskElement != null && !currentTaskElement.IsCompleted)
            {
                CompleteTask();
            }
        }
    }

    private void CompleteTask()
    {
        taskCompleted = true;

        currentTaskElement.MarkCompleted();
        roomManager.CheckPuzzleProgress();

        SpawnBox();

        Debug.Log("EngineManagerRed task completed: box spawned.");
    }

    private void SpawnBox()
    {
        if (boxPrefab != null)
        {
            Instantiate(boxPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Box prefab is not assigned in EngineManagerRed.");
        }
    }
}
