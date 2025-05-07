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
            unlockFrequencyText.text = $"Unlock Freq.: {unlockFrequency:F2}";
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
        // If the door is unlocked or the player is not nearby...
        if (unlocked || !playerNearby)
        {
            holdTimer = 0f;
            // Ensure door mode is off.
            if (playerTuner != null)
                playerTuner.SetGoodSignalActive(false, -1);
            if (statusLockText != null)
            {
                statusLockText.text = unlocked ? "UNLOCKED" : "LOCKED";
            }
            return;
        }

        // At this point, the player is nearby. Ensure door mode is active.
        if (playerTuner != null)
        {
            playerTuner.SetGoodSignalActive(true, unlockFrequency);
        }

        // Now check if the current frequency is within tolerance for unlocking.
        if (Mathf.Abs(playerTuner.currentFrequency - unlockFrequency) <= tolerance)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= requiredHoldTime)
            {
                UnlockDoor();
            }
        }
        else
        {
            // If the frequency isn't matching, simply reset the timer.
            // DON'T clear the door mode here because the player is still near the door.
            holdTimer = 0f;
        }

        // Update status text continuously.
        if (statusLockText != null)
        {
            statusLockText.text = unlocked ? "UNLOCKED" : "LOCKED";
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        // Attempt initial tuner lookup
        if (playerTuner == null)
            playerTuner = other.GetComponentInChildren<SignalTuner>();

        if (playerTuner != null)
            playerTuner.SetGoodSignalActive(true, unlockFrequency);
    }

    private void OnTriggerStay(Collider other)
    {
        // This catches the case where tuner is equipped *after* you enter
        if (playerNearby && playerTuner == null && other.CompareTag("Player"))
        {
            playerTuner = other.GetComponentInChildren<SignalTuner>();
            if (playerTuner != null)
                playerTuner.SetGoodSignalActive(true, unlockFrequency);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        holdTimer = 0f;
        if (playerTuner != null)
            playerTuner.SetGoodSignalActive(false, -1f);

        // Optionally clear tuner reference so we’ll re-find next time
        playerTuner = null;
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

    /// <summary>
    /// Exposes whether the door has been unlocked yet.
    /// </summary>
    public bool IsUnlocked
    {
        get { return unlocked; }
    }
}
