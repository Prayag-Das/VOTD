using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController2 : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject[] roomPrefabs; // Set 3 room prefabs here in Inspector

    [Header("Tube Prefabs")]
    public GameObject[] tubePrefabs; // Set 2 tube prefabs here in Inspector

    [Header("Generation Settings")]
    public int maxRooms = 18;
    public float roomOffset = 20.0f;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private List<GameObject> spawnedTubes = new List<GameObject>();
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

        SceneManager.Instance.SetCellOccupied(0, 0, true);
        filledCoordinates.Add(startCoord);

        spawnedRooms.Add(this.gameObject); // This room is the origin

        StartCoroutine(GenerateRooms());
    }

    void ResetGeneration()
    {
        // Destroy all rooms except the root
        foreach (GameObject room in spawnedRooms)
        {
            if (room != this.gameObject)
                Destroy(room);
        }

        // Destroy all tubes
        foreach (GameObject tube in spawnedTubes)
        {
            Destroy(tube);
        }

        spawnedRooms.Clear();
        spawnedTubes.Clear();
        filledCoordinates.Clear();

        SceneManager.Instance.ResetGrid();
        transform.position = originPosition;
    }

    IEnumerator GenerateRooms()
    {
        while (filledCoordinates.Count < maxRooms + 1)
        {
            int startIndex = filledCoordinates.Count > 1 ? 1 : 0;
            Vector2Int baseCoord = filledCoordinates[Random.Range(startIndex, filledCoordinates.Count)];
            Vector2Int[] neighbors = SceneManager.Instance.GetAvailableNeighbors(baseCoord);

            if (neighbors.Length == 0)
            {
                yield return null;
                continue;
            }

            Vector2Int newCoord = neighbors[Random.Range(0, neighbors.Length)];
            Vector3 roomWorldPos = originPosition + new Vector3(newCoord.x * roomOffset, 0, newCoord.y * roomOffset);

            // Randomly select room prefab
            GameObject randomRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

            // Randomly select rotation
            Quaternion randomRotation = Quaternion.Euler(0, GetRandom90Rotation(), 0);

            // Spawn the new room
            GameObject newRoom = Instantiate(randomRoomPrefab, roomWorldPos, randomRotation);
            spawnedRooms.Add(newRoom);

            SceneManager.Instance.SetCellOccupied(newCoord.x, newCoord.y, true);
            filledCoordinates.Add(newCoord);

            // Spawn a tube between baseCoord and newCoord
            Vector2Int direction = newCoord - baseCoord;
            Vector3 tubePos = Vector3.zero;
            Quaternion tubeRot = Quaternion.identity;

            if (direction == Vector2Int.up)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset, -0.07f, newCoord.y * roomOffset - 4.5f);
                tubeRot = Quaternion.Euler(0, 0, 0);
            }
            else if (direction == Vector2Int.down)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset, -0.07f, newCoord.y * roomOffset + 4.5f);
                tubeRot = Quaternion.Euler(0, 180, 0);
            }
            else if (direction == Vector2Int.right)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset - 4.5f, -0.07f, newCoord.y * roomOffset);
                tubeRot = Quaternion.Euler(0, 90, 0);
            }
            else if (direction == Vector2Int.left)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset + 4.5f, -0.07f, newCoord.y * roomOffset);
                tubeRot = Quaternion.Euler(0, -90, 0);
            }

            if (tubePrefabs.Length > 0)
            {
                GameObject randomTubePrefab = tubePrefabs[Random.Range(0, tubePrefabs.Length)];
                GameObject tube = Instantiate(randomTubePrefab, tubePos, tubeRot);
                spawnedTubes.Add(tube);
            }

            yield return new WaitForSeconds(0.1f); // Controls room+tube spawn rate
        }
    }

    int GetRandom90Rotation()
    {
        int[] possibleAngles = { -180, -90, 90, 180 };
        return possibleAngles[Random.Range(0, possibleAngles.Length)];
    }
}
