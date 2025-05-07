using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController2 : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject[] roomPrefabs;

    [Header("Special Rooms")]
    public GameObject[] specialRoomPrefabs;
    public GameObject[] specialTubePrefabs;

    [Header("Tube Prefabs")]
    public GameObject[] tubePrefabs;

    [Header("Blocker Prefabs")]
    public GameObject[] blockerPrefabs;

    [Header("Door Prefab")]
    public GameObject doorPrefab;

    [Header("Generation Settings")]
    public int maxRooms = 18;
    public float roomOffset = 20.0f;
    public float spawnDelay = 0.02f;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private List<GameObject> spawnedTubes = new List<GameObject>();
    private List<GameObject> spawnedBlockers = new List<GameObject>();
    private List<GameObject> spawnedDoors = new List<GameObject>();
    private List<Vector2Int> filledCoordinates = new List<Vector2Int>();
    private Dictionary<Vector2Int, bool[]> wallHandled = new Dictionary<Vector2Int, bool[]>();
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
        MapManager.Instance.ResetGrid();
        filledCoordinates.Clear();
        wallHandled.Clear();
        connectedRooms.Clear();

        MapManager.Instance.SetCellOccupied(0, 0, true);
        filledCoordinates.Add(startCoord);
        wallHandled[startCoord] = new bool[4];

        spawnedRooms.Add(this.gameObject);

        StartCoroutine(GenerateRooms());
    }

    void ResetGeneration()
    {
        foreach (GameObject room in spawnedRooms)
        {
            if (room != this.gameObject)
                Destroy(room);
        }

        foreach (GameObject tube in spawnedTubes) Destroy(tube);
        foreach (GameObject blocker in spawnedBlockers) Destroy(blocker);
        foreach (GameObject door in spawnedDoors) Destroy(door);

        spawnedRooms.Clear();
        spawnedTubes.Clear();
        spawnedBlockers.Clear();
        spawnedDoors.Clear();
        filledCoordinates.Clear();
        wallHandled.Clear();
        connectedRooms.Clear();

        MapManager.Instance.ResetGrid();
        transform.position = originPosition;
    }

    IEnumerator GenerateRooms()
    {
        int specialIndex = 0;

        while (filledCoordinates.Count < maxRooms + 1)
        {
            int startIndex = filledCoordinates.Count > 1 ? 1 : 0;

            // Exclude special room coords from baseCoord selection
            List<Vector2Int> candidateCoords = new List<Vector2Int>();
            for (int i = startIndex; i < filledCoordinates.Count; i++)
            {
                Vector2Int candidate = filledCoordinates[i];
                if (!MapManager.Instance.IsSpecialCoord(candidate))
                    candidateCoords.Add(candidate);
            }

            if (candidateCoords.Count == 0)
            {
                yield return null;
                continue;
            }

            Vector2Int baseCoord = candidateCoords[Random.Range(0, candidateCoords.Count)];
            Vector2Int[] neighbors = MapManager.Instance.GetAvailableNeighbors(baseCoord);

            if (neighbors.Length == 0)
            {
                yield return null;
                continue;
            }

            Vector2Int newCoord = neighbors[Random.Range(0, neighbors.Length)];
            Vector3 roomWorldPos = originPosition + new Vector3(newCoord.x * roomOffset, 0, newCoord.y * roomOffset);

            GameObject roomPrefab;
            Quaternion rotation;

            bool isSpecial = ((filledCoordinates.Count + 1) % 3 == 0) && specialIndex < specialRoomPrefabs.Length;

            if (isSpecial)
            {
                roomPrefab = specialRoomPrefabs[specialIndex];
                rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
                rotation = Quaternion.Euler(0, GetRandom90Rotation(), 0);
            }

            GameObject newRoom = Instantiate(roomPrefab, roomWorldPos, rotation);
            spawnedRooms.Add(newRoom);

            MapManager.Instance.SetCellOccupied(newCoord.x, newCoord.y, true);
            filledCoordinates.Add(newCoord);
            wallHandled[newCoord] = new bool[4];

            // Register special room state
            if (isSpecial)
            {
                if (!MapManager.Instance.Task3RoomSpawned)
                {
                    MapManager.Instance.Task3RoomSpawned = true;
                    MapManager.Instance.Task3Coord = newCoord;
                }
                else if (!MapManager.Instance.Task4RoomSpawned)
                {
                    MapManager.Instance.Task4RoomSpawned = true;
                    MapManager.Instance.Task4Coord = newCoord;
                }
            }

            Vector2Int tubeDir = newCoord - baseCoord;
            Vector3 tubePos = Vector3.zero;
            Quaternion tubeRot = Quaternion.identity;

            if (tubeDir == Vector2Int.up)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset, -0.07f, newCoord.y * roomOffset - 4.5f);
                tubeRot = Quaternion.Euler(0, 0, 0);
            }
            else if (tubeDir == Vector2Int.down)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset, -0.07f, newCoord.y * roomOffset + 4.5f);
                tubeRot = Quaternion.Euler(0, 180, 0);
            }
            else if (tubeDir == Vector2Int.right)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset - 4.5f, -0.07f, newCoord.y * roomOffset);
                tubeRot = Quaternion.Euler(0, 90, 0);
            }
            else if (tubeDir == Vector2Int.left)
            {
                tubePos = originPosition + new Vector3(newCoord.x * roomOffset + 4.5f, -0.07f, newCoord.y * roomOffset);
                tubeRot = Quaternion.Euler(0, -90, 0);
            }

            if (isSpecial && specialIndex < specialTubePrefabs.Length)
            {
                GameObject tube = Instantiate(specialTubePrefabs[specialIndex], tubePos, tubeRot);
                spawnedTubes.Add(tube);
            }
            else if (tubePrefabs.Length > 0)
            {
                GameObject randomTube = tubePrefabs[Random.Range(0, tubePrefabs.Length)];
                GameObject tube = Instantiate(randomTube, tubePos, tubeRot);
                spawnedTubes.Add(tube);

                connectedRooms.Add((baseCoord, newCoord));
                connectedRooms.Add((newCoord, baseCoord));

                // 1/3 chance to spawn door
                if (doorPrefab != null && Random.Range(0, 3) == 0)
                {
                    Vector3 doorPos = originPosition + new Vector3(baseCoord.x * roomOffset, 0, baseCoord.y * roomOffset);
                    Quaternion doorRot = Quaternion.identity;

                    if (tubeDir == Vector2Int.up) doorRot = Quaternion.Euler(0, 0, 0);
                    else if (tubeDir == Vector2Int.down) doorRot = Quaternion.Euler(0, 180, 0);
                    else if (tubeDir == Vector2Int.left) doorRot = Quaternion.Euler(0, -90, 0);
                    else if (tubeDir == Vector2Int.right) doorRot = Quaternion.Euler(0, 90, 0);

                    GameObject door = Instantiate(doorPrefab, doorPos, doorRot);
                    spawnedDoors.Add(door);
                }
            }

            if (isSpecial)
            {
                specialIndex++; // only once per special room
            }

            yield return new WaitForSeconds(spawnDelay);
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


            // UP (+Y)
            if (!wallHandled[coord][2])
            {
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
                wallHandled[coord][2] = true;
                if (wallHandled.ContainsKey(up)) wallHandled[up][3] = true;
            }


            // DOWN (-Y)
            if (!wallHandled[coord][3])
            {
                if (!filledCoordinates.Contains(down))
                {
                    if (!(coord.x == 0 && coord.y == 0))
                    {
                        yield return SpawnBlocker(basePos + new Vector3(0, 0, -3.75f), Quaternion.Euler(0, -90, 0));
                    }
                }
                else if (!connectedRooms.Contains((coord, down)))
                {
                    yield return SpawnBlocker(basePos + new Vector3(0, 0, -3.75f), Quaternion.Euler(0, -90, 0));
                    Vector3 neighborPos = originPosition + new Vector3(down.x * roomOffset, 0, down.y * roomOffset);
                    yield return SpawnBlocker(neighborPos + new Vector3(0, 0, 3.75f), Quaternion.Euler(0, 90, 0));
                }
                wallHandled[coord][3] = true;
                if (wallHandled.ContainsKey(down)) wallHandled[down][2] = true;
            }


            // LEFT (-X)
            if (!wallHandled[coord][1])
            {
                if (!filledCoordinates.Contains(left))
                {
                    if (!(coord.x == 0 && coord.y == 0))
                    {
                        yield return SpawnBlocker(basePos + new Vector3(-3.75f, 0, 0), Quaternion.Euler(0, 0, 0));
                    }
                }
                else if (!connectedRooms.Contains((coord, left)))
                {
                    yield return SpawnBlocker(basePos + new Vector3(-3.75f, 0, 0), Quaternion.Euler(0, 0, 0));
                    Vector3 neighborPos = originPosition + new Vector3(left.x * roomOffset, 0, left.y * roomOffset);
                    yield return SpawnBlocker(neighborPos + new Vector3(3.75f, 0, 0), Quaternion.Euler(0, 180, 0));
                }
                wallHandled[coord][1] = true;
                if (wallHandled.ContainsKey(left)) wallHandled[left][0] = true;
            }


            // RIGHT (+X)
            if (!wallHandled[coord][0])
            {
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
                wallHandled[coord][0] = true;
                if (wallHandled.ContainsKey(right)) wallHandled[right][1] = true;
            }


            yield return new WaitForSeconds(spawnDelay); // ✅ NEW: use spawnDelay variable
        }
    }

    IEnumerator SpawnBlocker(Vector3 pos, Quaternion rot)
    {
        if (blockerPrefabs.Length == 0) yield break;
        GameObject randomBlocker = blockerPrefabs[Random.Range(0, blockerPrefabs.Length)];
        GameObject blocker = Instantiate(randomBlocker, pos, rot);
        spawnedBlockers.Add(blocker);
        yield return null;
    }

    int GetRandom90Rotation()
    {
        int[] possibleAngles = { 0, -90, 90, 180 };
        return possibleAngles[Random.Range(0, possibleAngles.Length)];
    }
}