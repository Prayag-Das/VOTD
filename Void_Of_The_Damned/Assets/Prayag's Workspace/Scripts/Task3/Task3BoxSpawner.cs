using UnityEngine;
using System.Collections;
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

    private void Start()
    {
        StartCoroutine(SpawnBoxesAfterDelay());
    }

    private IEnumerator SpawnBoxesAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (currentTaskElement != null && currentTaskElement.IsCompleted)
        {
            taskCompleted = true;
            yield break;
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

        for (int i = 0; i < availableSpawns.Count; i++)
        {
            Transform temp = availableSpawns[i];
            int randomIndex = Random.Range(i, availableSpawns.Count);
            availableSpawns[i] = availableSpawns[randomIndex];
            availableSpawns[randomIndex] = temp;
        }

        Instantiate(redBoxPrefab, availableSpawns[0].position, Quaternion.identity, transform);
        Instantiate(greenBoxPrefab, availableSpawns[1].position, Quaternion.identity, transform);
        Instantiate(blueBoxPrefab, availableSpawns[2].position, Quaternion.identity, transform);

        taskCompleted = true;
        currentTaskElement.MarkCompleted();
        roomManager.CheckPuzzleProgress();

        Debug.Log("Task3BoxSpawner: Boxes spawned and task marked complete.");
    }
}
