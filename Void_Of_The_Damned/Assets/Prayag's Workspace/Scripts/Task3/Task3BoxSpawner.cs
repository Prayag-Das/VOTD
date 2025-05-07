using UnityEngine;
using System.Collections.Generic;

public class Task3BoxSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawners;

    [Header("Box Prefabs")]
    [SerializeField] private GameObject redBoxPrefab;
    [SerializeField] private GameObject greenBoxPrefab;
    [SerializeField] private GameObject blueBoxPrefab;

    [Header("Task Dependencies")]
    [SerializeField] private PuzzleRoomManager roomManager;
    [SerializeField] private PuzzleElement currentTaskElement;

    private bool taskCompleted = false;

    private void Awake()
    {
        if (currentTaskElement != null && currentTaskElement.IsCompleted)
        {
            taskCompleted = true;
            return;
        }

        SpawnBoxes();
    }

    private void SpawnBoxes()
    {
        if (spawners.Count < 3)
        {
            Debug.LogWarning("Not enough spawn points assigned for Task3BoxSpawner.");
            return;
        }

        List<Transform> availableSpawns = new List<Transform>(spawners);

        // Shuffle the list randomly
        for (int i = 0; i < availableSpawns.Count; i++)
        {
            Transform temp = availableSpawns[i];
            int randomIndex = Random.Range(i, availableSpawns.Count);
            availableSpawns[i] = availableSpawns[randomIndex];
            availableSpawns[randomIndex] = temp;
        }

        // Spawn each box at a unique spawn point
        Instantiate(redBoxPrefab, availableSpawns[0].position, Quaternion.identity);
        Instantiate(greenBoxPrefab, availableSpawns[1].position, Quaternion.identity);
        Instantiate(blueBoxPrefab, availableSpawns[2].position, Quaternion.identity);

        taskCompleted = true;
        currentTaskElement.MarkCompleted();
        roomManager.CheckPuzzleProgress();

        Debug.Log("Task3BoxSpawner: Boxes spawned and task marked complete.");
    }
}
