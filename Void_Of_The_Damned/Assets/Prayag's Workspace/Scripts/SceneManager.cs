using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public const int gridSize = 25;
    private bool[,] grid = new bool[gridSize, gridSize];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGrid()
    {
        grid = new bool[gridSize, gridSize];
    }

    public bool IsCellOccupied(int x, int y)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) return true;
        return grid[x, y];
    }

    public void SetCellOccupied(int x, int y, bool value)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) return;
        grid[x, y] = value;
    }

    public Vector2Int[] GetAvailableNeighbors(Vector2Int coord)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        var available = new System.Collections.Generic.List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int neighbor = coord + dir;
            if (!IsCellOccupied(neighbor.x, neighbor.y))
                available.Add(neighbor);
        }
        return available.ToArray();
    }
}
