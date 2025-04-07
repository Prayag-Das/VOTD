using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject roomPrefab;
    public int maxRooms = 10;
    public float roomOffset = 14.7f;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private List<Vector2Int> filledCoordinates = new List<Vector2Int>();
    private Vector3 originPosition;

    void Start()
    {
        originPosition = transform.position;
        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGeneration();
            Generate();
        }
    }

    void Generate()
    {
        Vector2Int startCoord = new Vector2Int(0, 0);
        SceneManager.Instance.ResetGrid();
        filledCoordinates.Clear();

        // Mark initial room
        SceneManager.Instance.SetCellOccupied(0, 0, true);
        filledCoordinates.Add(startCoord);

        // Add the initial room to tracking list (this object)
        spawnedRooms.Add(this.gameObject);

        StartCoroutine(GenerateRooms());
    }

    void ResetGeneration()
    {
        // Destroy all spawned rooms except the one holding this controller
        foreach (GameObject room in spawnedRooms)
        {
            if (room != this.gameObject)
                Destroy(room);
        }

        spawnedRooms.Clear();
        filledCoordinates.Clear();
        SceneManager.Instance.ResetGrid();

        // Reset the origin
        transform.position = originPosition;
    }

    IEnumerator GenerateRooms()
    {
        while (filledCoordinates.Count < maxRooms)
        {
            Vector2Int baseCoord = filledCoordinates[Random.Range(0, filledCoordinates.Count)];
            Vector2Int[] neighbors = SceneManager.Instance.GetAvailableNeighbors(baseCoord);

            if (neighbors.Length == 0)
            {
                yield return null;
                continue;
            }

            Vector2Int newCoord = neighbors[Random.Range(0, neighbors.Length)];
            Vector3 spawnPos = originPosition + new Vector3(newCoord.x * roomOffset, 0, newCoord.y * roomOffset);

            GameObject newRoom = Instantiate(roomPrefab, spawnPos, Quaternion.identity);
            spawnedRooms.Add(newRoom);

            SceneManager.Instance.SetCellOccupied(newCoord.x, newCoord.y, true);
            filledCoordinates.Add(newCoord);

            yield return new WaitForSeconds(0.1f); // Optional delay for effect
        }
    }
}
