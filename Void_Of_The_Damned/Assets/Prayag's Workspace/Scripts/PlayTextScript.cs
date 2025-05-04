using UnityEngine;
using TMPro;
using System.Collections;

public class PlayTextScript : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float waitBetweenFades = 0.5f;

    private bool isFadingOut = false;
    private bool spacePressed = false;

    private void Start()
    {
        if (playText != null)
        {
            Color color = playText.color;
            color.a = 0f;
            playText.color = color;
        }

        StartCoroutine(BlinkText());
    }

    private void Update()
    {
        if (!spacePressed && Input.GetKeyDown(KeyCode.Space))
        {
            spacePressed = true;
        }
    }

    private IEnumerator BlinkText()
    {
        while (!spacePressed)
        {
            // Fade In
            yield return StartCoroutine(FadeTextAlpha(0f, 1f));

            // Wait
            yield return new WaitForSeconds(waitBetweenFades);

            // Fade Out
            yield return StartCoroutine(FadeTextAlpha(1f, 0f));

            // Wait
            yield return new WaitForSeconds(waitBetweenFades);
        }

        // Once Space is pressed, fade out and stop
        yield return StartCoroutine(FadeTextAlpha(playText.color.a, 0f));
    }

    private IEnumerator FadeTextAlpha(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = playText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            color.a = alpha;
            playText.color = color;
            yield return null;
        }

        color.a = endAlpha;
        playText.color = color;
    }
}
