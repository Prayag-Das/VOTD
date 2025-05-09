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
    [SerializeField] private float moveSpeed = 0.8f;
    [SerializeField] private float stopZ = -40f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource introAudioSource;

    private bool isMoving = false;
    private bool hasStopped = false;
    private bool isCutscenePlaying = false;
    private bool isSkipping = false;

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

        if (!isSkipping && Input.GetKeyDown(KeyCode.Space))
        {
            isSkipping = true;
            StartCoroutine(SkipCutscene());
        }

        if (isMoving && !hasStopped)
        {
            transform.position += new Vector3(0f, 0f, moveSpeed * Time.deltaTime);

            if (transform.position.z >= stopZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, stopZ);
                isMoving = false;
                hasStopped = true;

                if (!isSkipping)
                {
                    StartCoroutine(HandleStopFadeOut());
                }
            }
        }
    }

    private IEnumerator PlayCutscene()
    {
        isCutscenePlaying = true;
        hasStopped = false;
        isMoving = false;

        transform.position = initialPosition;

        if (introAudioSource != null)
        {
            introAudioSource.Stop();
            introAudioSource.Play();
        }

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeScreen(1f, 0f));

        isMoving = true;
    }

    private IEnumerator HandleStopFadeOut()
    {
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeScreen(0f, 1f));
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Prayag-Testing-Area");
    }

    private IEnumerator SkipCutscene()
    {
        yield return StartCoroutine(FadeScreen(fadeImage.color.a, 1f));
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
