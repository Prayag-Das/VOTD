using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScript : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string nextSceneName = "Intro-Cutscene-Prototype";

    private bool isTransitioning = false;

    private void Start()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f; // Start fully visible (no black screen)
            fadeImage.color = color;
        }
    }

    private void Update()
    {
        if (isTransitioning) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TransitionToNextScene());
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;

        // Fade to black
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Short wait to ensure fade is complete (optional)
        yield return new WaitForSeconds(0.1f);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha)
    {
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

        // Ensure final alpha is set perfectly
        color.a = endAlpha;
        fadeImage.color = color;
    }
}
