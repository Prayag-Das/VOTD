using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController2 : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject[] roomPrefabs;

    [Header("Tube Prefabs")]
    public GameObject[] tubePrefabs;

    [Header("Blocker Prefabs")]
    public GameObject[] blockerPrefabs;

    [Header("Generation Settings")]
    public int maxRooms = 18;
    public float roomOffset = 20.0f;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private List<GameObject> spawnedTubes = new List<GameObject>();
    private List<GameObject> spawnedBlockers = new List<GameObject>();
    private List<Vector2Int> filledCoordinates = new List<Vector2Int>();
    private HashSet<(Vector2Int, Vector2Int)> connectedRooms = new HashSet<(Vector2Int, Vector2Int)>();
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
        connectedRooms.Clear();

        SceneManager.Instance.SetCellOccupied(0, 0, true);
        filledCoordinates.Add(startCoord);

        spawnedRooms.Add(this.gameObject); // Root room

        StartCoroutine(GenerateRooms());
    }

    void ResetGeneration()
    {
        foreach (GameObject room in spawnedRooms)
        {
            if (room != this.gameObject)
                Destroy(room);
        }

        foreach (GameObject tube in spawnedTubes)
        {
            Destroy(tube);
        }

        foreach (GameObject blocker in spawnedBlockers)
        {
            Destroy(blocker);
        }

        spawnedRooms.Clear();
        spawnedTubes.Clear();
        spawnedBlockers.Clear();
        filledCoordinates.Clear();
        connectedRooms.Clear();

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

            GameObject randomRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            Quaternion randomRotation = Quaternion.Euler(0, GetRandom90Rotation(), 0);

            GameObject newRoom = Instantiate(randomRoomPrefab, roomWorldPos, randomRotation);
            spawnedRooms.Add(newRoom);

            SceneManager.Instance.SetCellOccupied(newCoord.x, newCoord.y, true);
            filledCoordinates.Add(newCoord);

            // Spawn tube
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

                // RECORD connection
                connectedRooms.Add((baseCoord, newCoord));
                connectedRooms.Add((newCoord, baseCoord));
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(SpawnBlockers());
    }

    IEnumerator SpawnBlockers()
    {
        foreach (Vector2Int coord in filledCoordinates)
        {
            Vector3 basePos = originPosition + new Vector3(coord.x * roomOffset, 0, coord.y * roomOffset);

            Vector2Int up = coord + Vector2Int.up;
            Vector2Int down = coord + Vector2Int.down;
            Vector2Int left = coord + Vector2Int.left;
            Vector2Int right = coord + Vector2Int.right;

            // Up
            if (!filledCoordinates.Contains(up))
            {
                yield return SpawnBlocker(basePos + new Vector3(0, 0, 3.75f), Quaternion.Euler(0, 90, 0));
            }
            else if (!connectedRooms.Contains((coord, up)))
            {
                yield return SpawnBlocker(basePos + new Vector3(0, 0, 3.75f), Quaternion.Euler(0, 90, 0));
                Vector3 neighborPos = originPosition + new Vector3(up.x * roomOffset, 0, up.y * roomOffset);
                yield return SpawnBlocker(neighborPos + new Vector3(0, 0, -3.75f), Quaternion.Euler(0, -90, 0));
            }

            // Down
            if (!filledCoordinates.Contains(down))
            {
                yield return SpawnBlocker(basePos + new Vector3(0, 0, -3.75f), Quaternion.Euler(0, -90, 0));
            }
            else if (!connectedRooms.Contains((coord, down)))
            {
                yield return SpawnBlocker(basePos + new Vector3(0, 0, -3.75f), Quaternion.Euler(0, -90, 0));
                Vector3 neighborPos = originPosition + new Vector3(down.x * roomOffset, 0, down.y * roomOffset);
                yield return SpawnBlocker(neighborPos + new Vector3(0, 0, 3.75f), Quaternion.Euler(0, 90, 0));
            }

            // Left
            if (!filledCoordinates.Contains(left))
            {
                yield return SpawnBlocker(basePos + new Vector3(-3.75f, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            else if (!connectedRooms.Contains((coord, left)))
            {
                yield return SpawnBlocker(basePos + new Vector3(-3.75f, 0, 0), Quaternion.Euler(0, 0, 0));
                Vector3 neighborPos = originPosition + new Vector3(left.x * roomOffset, 0, left.y * roomOffset);
                yield return SpawnBlocker(neighborPos + new Vector3(3.75f, 0, 0), Quaternion.Euler(0, 180, 0));
            }

            // Right
            if (!filledCoordinates.Contains(right))
            {
                yield return SpawnBlocker(basePos + new Vector3(3.75f, 0, 0), Quaternion.Euler(0, 180, 0));
            }
            else if (!connectedRooms.Contains((coord, right)))
            {
                yield return SpawnBlocker(basePos + new Vector3(3.75f, 0, 0), Quaternion.Euler(0, 180, 0));
                Vector3 neighborPos = originPosition + new Vector3(right.x * roomOffset, 0, right.y * roomOffset);
                yield return SpawnBlocker(neighborPos + new Vector3(-3.75f, 0, 0), Quaternion.Euler(0, 0, 0));
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SpawnBlocker(Vector3 pos, Quaternion rot)
    {
        // Skip spawning blocker at (0, 0, -3.75)
        if (Mathf.Approximately(pos.x, 0f) && Mathf.Approximately(pos.z, -3.75f))
        {
            yield break;
        }

        if (blockerPrefabs.Length == 0) yield break;
        GameObject randomBlocker = blockerPrefabs[Random.Range(0, blockerPrefabs.Length)];
        GameObject blocker = Instantiate(randomBlocker, pos, rot);
        spawnedBlockers.Add(blocker);
        yield return null;
    }

    int GetRandom90Rotation()
    {
        int[] possibleAngles = { -180, -90, 90, 180 };
        return possibleAngles[Random.Range(0, possibleAngles.Length)];
    }
}
