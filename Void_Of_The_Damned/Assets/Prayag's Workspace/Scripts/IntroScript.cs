using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroScript : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f; // Constant movement speed
    [SerializeField] private float stopZ = -40f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource introAudioSource;    // AudioSource to play when cutscene starts

    private bool isMoving = false;
    private bool hasStopped = false;
    private bool isCutscenePlaying = false;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y, -45f);

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        StartCoroutine(PlayCutscene());
    }

    private void Update()
    {
        if (!isCutscenePlaying) return;
        if (!isMoving || hasStopped) return;

        transform.position += new Vector3(0f, 0f, moveSpeed * Time.deltaTime);

        if (transform.position.z >= stopZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, stopZ);
            isMoving = false;
            hasStopped = true;
            StartCoroutine(HandleStopFadeOut());
        }
    }

    private IEnumerator PlayCutscene()
    {
        isCutscenePlaying = true;
        hasStopped = false;
        isMoving = false;

        // Reset fade to full black immediately
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        transform.position = initialPosition;

        // Play audio at the beginning of the sequence
        if (introAudioSource != null)
        {
            introAudioSource.Stop();
            introAudioSource.Play();
        }

        // Stay black for 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Fade in from black
        yield return StartCoroutine(FadeScreen(1f, 0f));

        // Start moving at constant speed
        isMoving = true;
    }

    private IEnumerator HandleStopFadeOut()
    {
        // After stopping, wait 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Fade out to black
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Short wait to ensure fade and audio finish
        yield return new WaitForSeconds(1.0f);

        // Load the next scene
        SceneManager.LoadScene("Prayag-Testing-Area");
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

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
