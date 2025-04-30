using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Grid Settings")]
    public const int gridSize = 19;
    private bool[,] grid = new bool[gridSize, gridSize];

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Camera Settings")]
    [SerializeField] private Camera primaryCamera;
    [SerializeField] private Camera stationViewCamera;

    private bool isFading = false;
    private bool isCutscenePlaying = false; // To prevent multiple scene loads

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
            return;
        }

        // Start with fade fully black
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        // Fade in from black
        StartCoroutine(FadeScreen(1f, 0f));

        // Set primary camera active at start
        if (primaryCamera != null && stationViewCamera != null)
        {
            primaryCamera.enabled = true;
            stationViewCamera.enabled = false;
        }
    }

    void Update()
    {
        HandleCameraSwap();
        HandleSceneReturn();
    }

    private void HandleCameraSwap()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (primaryCamera != null && stationViewCamera != null)
            {
                bool primaryActive = primaryCamera.enabled;
                primaryCamera.enabled = !primaryActive;
                stationViewCamera.enabled = primaryActive;
            }
        }
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
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Ensure final alpha
        color.a = endAlpha;
        fadeImage.color = color;

        isFading = false;
    }

    private IEnumerator FadeOutAndReturnToTitle()
    {
        isCutscenePlaying = true;

        // Fade to black
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Short wait to ensure the fade completes
        yield return new WaitForSeconds(0.1f);

        // Load the Title Screen scene
        SceneManager.LoadScene("Title-Screen-Prototype");
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
