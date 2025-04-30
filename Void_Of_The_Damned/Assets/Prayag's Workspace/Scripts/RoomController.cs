using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject tubePrefab;
    public int maxRooms = 18;
    public float roomOffset = 14.7f;

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
        MapManager.Instance.ResetGrid();
        filledCoordinates.Clear();

        MapManager.Instance.SetCellOccupied(0, 0, true);
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

        MapManager.Instance.ResetGrid();
        transform.position = originPosition;
    }

    IEnumerator GenerateRooms()
    {
        while (filledCoordinates.Count < maxRooms + 1)
        {
            int startIndex = filledCoordinates.Count > 1 ? 1 : 0;
            Vector2Int baseCoord = filledCoordinates[Random.Range(startIndex, filledCoordinates.Count)];
            Vector2Int[] neighbors = MapManager.Instance.GetAvailableNeighbors(baseCoord);

            if (neighbors.Length == 0)
            {
                yield return null;
                continue;
            }

            Vector2Int newCoord = neighbors[Random.Range(0, neighbors.Length)];
            Vector3 roomWorldPos = originPosition + new Vector3(newCoord.x * roomOffset, 0, newCoord.y * roomOffset);

            GameObject newRoom = Instantiate(roomPrefab, roomWorldPos, Quaternion.identity);
            spawnedRooms.Add(newRoom);

            MapManager.Instance.SetCellOccupied(newCoord.x, newCoord.y, true);
            filledCoordinates.Add(newCoord);

            // Spawn a tube between baseCoord and newCoord
            Vector2Int direction = newCoord - baseCoord;
            Vector3 tubePos = Vector3.zero;
            Quaternion tubeRot = Quaternion.identity;

            if (direction == Vector2Int.up)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset, 0, newCoord.y * roomOffset - 7.4f);
                tubeRot = Quaternion.Euler(0, 0, 0);
            }
            else if (direction == Vector2Int.down)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset, 0, newCoord.y * roomOffset + 7.4f);
                tubeRot = Quaternion.Euler(0, 0, 0);
            }
            else if (direction == Vector2Int.right)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset - 7.4f, 0, newCoord.y * roomOffset);
                tubeRot = Quaternion.Euler(0, 90, 0);
            }
            else if (direction == Vector2Int.left)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset + 7.4f, 0, newCoord.y * roomOffset);
                tubeRot = Quaternion.Euler(0, 90, 0);
            }

            if (tubePrefab != null)
            {
                GameObject tube = Instantiate(tubePrefab, tubePos, tubeRot);
                spawnedTubes.Add(tube);
            }

            yield return new WaitForSeconds(0.1f); // Controls room+tube spawn rate
        }
    }
}
