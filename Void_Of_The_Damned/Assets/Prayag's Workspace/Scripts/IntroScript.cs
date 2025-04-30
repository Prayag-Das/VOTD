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
    [SerializeField] private float acceleration = 4f;         // Faster initial acceleration
    [SerializeField] private float maxSpeed = 10f;            // Faster max speed
    [SerializeField] private float decelerationMultiplier = 5f; // Much faster deceleration
    [SerializeField] private float decelerationStartZ = -42f;
    [SerializeField] private float stopZ = -40f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource introAudioSource;    // AudioSource to play when cutscene starts

    private float currentSpeed = 0f;
    private bool isMoving = false;
    private bool hasStopped = false;
    private bool hasStartedMoving = false;
    private bool isCutscenePlaying = false;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y, -90f);

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

        float zPos = transform.position.z;

        if (zPos >= decelerationStartZ)
        {
            currentSpeed -= acceleration * decelerationMultiplier * Time.deltaTime;
        }
        else
        {
            currentSpeed += acceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        transform.position += new Vector3(0f, 0f, currentSpeed * Time.deltaTime);

        if (transform.position.z >= stopZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, stopZ);
            currentSpeed = 0f;
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
        hasStartedMoving = false;

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

        // Start moving
        isMoving = true;
        hasStartedMoving = true;
    }

    private IEnumerator HandleStopFadeOut()
    {
        // After stopping, wait 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Fade out to black
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Short wait to ensure fade finishes
        yield return new WaitForSeconds(0.1f);

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
