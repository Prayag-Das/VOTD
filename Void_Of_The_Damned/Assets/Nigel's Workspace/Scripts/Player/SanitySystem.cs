using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float sanityDecayRate = 0.2f;   // Sanity drains over time
    [SerializeField] private float cleanseAmount = 30f;    // Amount restored by Hazard Showers
    [SerializeField] private float insanityThreshold = 0f; // Triggers insanity when sanity hits zero

    [Header("Insanity Effects")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Transform playerTransform;

    [Header("References")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    private float currentSanity; // Tracks the player's sanity
    private bool isFading = false;


    private void Start()
    {
        currentSanity = maxSanity;

        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
        }
    }

    private void Update()
    {
        DecreaseSanityOverTime();
    }

    // Decreases sanity as time progresses
    private void DecreaseSanityOverTime()
    {
        if (currentSanity > insanityThreshold)
        {
            currentSanity -= sanityDecayRate * Time.deltaTime;
            //Debug.Log($"Sanity: {currentSanity}");

            if (currentSanity <= insanityThreshold && !isFading)
            {
                StartCoroutine(TriggerInsanity());
            }
        }
    }

    // Called when the player interacts with a cleansing object
    public void CleanseSanity()
    {
        currentSanity = Mathf.Min(currentSanity + cleanseAmount, maxSanity);
        Debug.Log($"Sanity Restored! Current Sanity: {currentSanity}");
    }

    // Trigger insanity logic (for now, just a debug message)
    private IEnumerator TriggerInsanity()
    {
        Debug.Log("Player has gone insane! Triggering insanity effects...");
        isFading = true;

        // Disable player movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Fade to black
        yield return StartCoroutine(FadeScreen(0f, 1f));

        // Teleport player
        if (playerTransform != null && respawnPoint != null)
        {
            playerTransform.position = respawnPoint.position;
        }

        // Fade back in
        yield return StartCoroutine(FadeScreen(1f, 0f));

        // Re-enable player movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }


        // Reset sanity
        currentSanity = maxSanity;
        isFading = false;
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                fadeColor.a = alpha;
                fadeImage.color = fadeColor;
                yield return null;
            }

            // Ensure final alpha is set
            fadeColor.a = endAlpha;
            fadeImage.color = fadeColor;
        }
    }

}
