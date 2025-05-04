using UnityEngine;
using System.Collections;

public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 50f;
    [SerializeField] private float oxygenDecayRate = 0.2f;
    [SerializeField] private float oxygenDepletedThreshold = 0f;

    private float currentOxygen;
    private bool isDepleted = false;

    private void Start()
    {
        ResetOxygen();
    }

    private void Update()
    {
        HandleOxygenDecay();

        // DEBUG: Show oxygen status in console while playing
         Debug.Log($"[OxygenSystem] Current Oxygen: {currentOxygen}/{maxOxygen}");
    }

    private void HandleOxygenDecay()
    {
        if (isDepleted) return;

        currentOxygen -= oxygenDecayRate * Time.deltaTime;

        if (currentOxygen <= oxygenDepletedThreshold)
        {
            currentOxygen = 0f;
            HandleOxygenDepleted();
        }
    }

    public void RefillOxygen(float amount)
    {
        currentOxygen = Mathf.Min(currentOxygen + amount, maxOxygen);
        Debug.Log($"[OxygenSystem] Oxygen Refilled! Current Oxygen: {currentOxygen}");
    }

    public void TriggerInstantOxygenLoss()
    {
        currentOxygen = 0f;
        HandleOxygenDepleted();
    }

    private void HandleOxygenDepleted()
    {
        if (!isDepleted)
        {
            isDepleted = true;
            Debug.Log("OXYGEN DEPLETED - Triggering Insanity...");

            // Find Sanity System and trigger instant insanity
            SanitySystem sanity = FindFirstObjectByType<SanitySystem>();
            if (sanity != null)
            {
                sanity.TriggerInstantInsanity();

                // Wait for sanity reset, then reset oxygen
                StartCoroutine(ResetOxygenAfterDelay(2f)); // <- Give sanity a little time (adjust if needed)
            }
            else
            {
                Debug.LogWarning("SanitySystem not found! Cannot trigger insanity.");
            }
        }
    }

    private IEnumerator ResetOxygenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ResetOxygen();
    }

    private void ResetOxygen()
    {
        currentOxygen = maxOxygen;
        isDepleted = false;
        Debug.Log("[OxygenSystem] Oxygen fully reset!");
    }

    public float GetCurrentOxygen() => currentOxygen;
    public float GetMaxOxygen() => maxOxygen;
}