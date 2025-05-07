using UnityEngine;

public class EngineControllerGreen : MonoBehaviour
{
    [Header("Task Dependencies")]
    [SerializeField] private PuzzleRoomManager roomManager;
    [SerializeField] private PuzzleElement currentTaskElement;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float spawnYOffset = 0.25f; // Height offset for spawn

    private bool taskCompleted = false;

    private void Start()
    {
        if (currentTaskElement != null && currentTaskElement.IsCompleted)
        {
            taskCompleted = true;
            SpawnBox();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (taskCompleted) return;

        if (other.CompareTag("Task-3-green"))
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

        Debug.Log("EngineManagerGreen task completed: box spawned.");
    }

    private void SpawnBox()
    {
        if (boxPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, spawnYOffset, 0);
            Instantiate(boxPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Box prefab is not assigned in EngineControllerGreen.");
        }
    }
}
