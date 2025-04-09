using TMPro;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    [Header("Unlock Settings")]
    public float unlockFrequency = 170f;
    public float requiredHoldTime = 5f;
    public float tolerance = 0.01f;

    private float holdTimer = 0f;
    private bool playerNearby = false;
    private bool unlocked = false;

    [Header("UI Elements")]
    public TMP_Text unlockFrequencyText;

    public TMP_Text statusLockText;

    [Header("Player Tuner Reference")]
    // Reference to the player's SignalTuner component.
    public SignalTuner playerTuner;

    [Header("Audio")]
    public AudioSource doorSfxSource;
    public AudioClip unlockClip;

    private void Start()
    {
        // Update the unlock frequency text.
        if (unlockFrequencyText != null)
        {
            unlockFrequencyText.text = $"Unlock Frequency: {unlockFrequency:F2}";
        }

        // Initially set door status as locked.
        if (statusLockText != null)
        {
            statusLockText.text = "LOCKED";
            statusLockText.color = Color.red;
        }
    }

    private void Update()
    {
        // Only proceed if the door is still locked and the player is nearby.
        if (unlocked || !playerNearby)
        {
            // Reset timer if the player moves away.
            holdTimer = 0f;

            // Tell tuner: not matching = stop good signal
            if (playerTuner != null)
                playerTuner.SetGoodSignalActive(false);

            // Update status text.
            if (statusLockText != null)
            {
                statusLockText.text = unlocked ? "UNLOCKED" : "LOCKED";
            }

            return;
        }

        // Check if the tuner's frequency is close enough to the door's unlock frequency.
        if (Mathf.Abs(playerTuner.currentFrequency - unlockFrequency) <= tolerance)
        {
            // Tell tuner: frequency matches play good signal
            if (playerTuner != null)
                playerTuner.SetGoodSignalActive(true);

            holdTimer += Time.deltaTime;
            if (holdTimer >= requiredHoldTime)
            {
                UnlockDoor();
            }
        }
        else
        {
            // Tell tuner: frequency does NOT match stop good signal
            if (playerTuner != null)
                playerTuner.SetGoodSignalActive(false);

            // Frequency does not match; reset the timer.
            holdTimer = 0f;
        }

        // Continuously update the status text.
        if (statusLockText != null)
        {
            statusLockText.text = unlocked ? "UNLOCKED" : "LOCKED";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            holdTimer = 0f;
        }
    }

    // Unlock the door (you can add animations, disable colliders, etc.)
    private void UnlockDoor()
    {
        unlocked = true;
        Debug.Log("Door Unlocked!");

        // Play the unlock sound once using the door's AudioSource.
        if (doorSfxSource != null && unlockClip != null)
        {
            doorSfxSource.PlayOneShot(unlockClip);
        }

        if (statusLockText != null)
        {
            statusLockText.text = "UNLOCKED";
            statusLockText.color = Color.green;
        }

        // Additional Stuff : Animation, Disable Collider, etc.
    }
}
