using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoScript : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f; // Start fully black
            fadeImage.color = color;
        }

        StartCoroutine(PlayLogoSequence());
    }

    private IEnumerator PlayLogoSequence()
    {
        // Stay black for 0.5 seconds first
        yield return new WaitForSeconds(0.5f);

        // Fade in from black
        yield return StartCoroutine(FadeScreen(1f, 0f));

        // Wait 2 seconds while logo/screen is visible
        yield return new WaitForSeconds(1f);

        // Fade back to black
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Short wait to ensure fade finishes
        yield return new WaitForSeconds(0.1f);

        // Load the next scene
        SceneManager.LoadScene("Title-Screen-Prototype");
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
