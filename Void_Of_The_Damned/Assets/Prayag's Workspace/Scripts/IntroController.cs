using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 2f;       // Increased for faster ramp-up
    [SerializeField] private float maxSpeed = 5f;           // Increased max speed
    [SerializeField] private float decelerationStartZ = -46f;
    [SerializeField] private float stopZ = -40f;

    private float currentSpeed = 0f;
    private bool isMoving = false;
    private bool hasStopped = false;

    private void Start()
    {
        // Fade starts fully black
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // Fade in from black
        yield return StartCoroutine(FadeScreen(1f, 0f));

        // Wait 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Start moving
        isMoving = true;

        // Wait for the camera to stop
        while (!hasStopped)
        {
            yield return null;
        }

        // Fade out
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Wait a second
        yield return new WaitForSeconds(1f);

        // Fade back in
        yield return StartCoroutine(FadeScreen(1f, 0f));
    }

    private void Update()
    {
        if (!isMoving || hasStopped) return;

        float zPos = transform.position.z;

        // Adjust speed based on position
        if (zPos >= decelerationStartZ)
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed += acceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // Move camera
        transform.position += new Vector3(0f, 0f, currentSpeed * Time.deltaTime);

        // Check if we've reached or passed stopZ
        if (transform.position.z >= stopZ)
        {
            // Snap to exact stopZ and stop movement
            transform.position = new Vector3(transform.position.x, transform.position.y, stopZ);
            currentSpeed = 0f;
            isMoving = false;
            hasStopped = true;
        }
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
