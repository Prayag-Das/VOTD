using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Grid Settings")]
    public const int gridSize = 19;
    private bool[,] grid = new bool[gridSize, gridSize];

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Text or Overlay Fade")]
    [SerializeField] private RawImage overlayImage;

    private bool isFading = false;
    private bool isCutscenePlaying = false;

    // ✅ Task room tracking
    public bool Task3RoomSpawned { get; set; } = false;
    public bool Task4RoomSpawned { get; set; } = false;
    public bool Task2RoomSpawned { get; set; } = false; // ✅ NEW
    public Vector2Int? Task3Coord { get; set; } = null;
    public Vector2Int? Task4Coord { get; set; } = null;
    public Vector2Int? Task2Coord { get; set; } = null; // ✅ NEW

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        if (overlayImage != null)
        {
            Color color = overlayImage.color;
            color.a = 1f;
            overlayImage.color = color;
        }

        StartCoroutine(FadeScreen(1f, 0f));
    }

    void Update()
    {
        HandleSceneReturn();
    }

    private void HandleSceneReturn()
    {
        if (isCutscenePlaying) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(FadeOutAndReturnToTitle());
        }
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha)
    {
        if (isFading) yield break;
        isFading = true;

        float elapsed = 0f;
        Color fadeColor = fadeImage.color;
        Color overlayColor = overlayImage != null ? overlayImage.color : Color.clear;
        float overlayStartAlpha = overlayImage != null ? overlayColor.a : 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = fadeColor;

            if (overlayImage != null && startAlpha < endAlpha)
            {
                overlayColor.a = Mathf.Lerp(overlayStartAlpha, 0f, t);
                overlayImage.color = overlayColor;
            }

            yield return null;
        }

        fadeColor.a = endAlpha;
        fadeImage.color = fadeColor;

        if (overlayImage != null && startAlpha < endAlpha)
        {
            overlayColor.a = 0f;
            overlayImage.color = overlayColor;
        }

        isFading = false;
    }

    private IEnumerator FadeOutAndReturnToTitle()
    {
        isCutscenePlaying = true;
        SceneManager.LoadScene("Title-Screen-Prototype");
        yield break;
    }

    public void ResetGrid()
    {
        grid = new bool[gridSize, gridSize];
        Task3RoomSpawned = false;
        Task4RoomSpawned = false;
        Task2RoomSpawned = false; // ✅ NEW
        Task3Coord = null;
        Task4Coord = null;
        Task2Coord = null; // ✅ NEW
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

        var available = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int neighbor = coord + dir;
            if (!IsCellOccupied(neighbor.x, neighbor.y))
                available.Add(neighbor);
        }
        return available.ToArray();
    }

    public bool IsSpecialCoord(Vector2Int coord)
    {
        return (Task3Coord != null && Task3Coord == coord) ||
               (Task4Coord != null && Task4Coord == coord) ||
               (Task2Coord != null && Task2Coord == coord); // ✅ NEW
    }

    public void MarkTask3Room(Vector2Int coord)
    {
        Task3RoomSpawned = true;
        Task3Coord = coord;
    }

    public void MarkTask4Room(Vector2Int coord)
    {
        Task4RoomSpawned = true;
        Task4Coord = coord;
    }

    public void MarkTask2Room(Vector2Int coord) // ✅ NEW
    {
        Task2RoomSpawned = true;
        Task2Coord = coord;
    }
}
