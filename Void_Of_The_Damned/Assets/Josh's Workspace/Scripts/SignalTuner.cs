using UnityEngine;
using TMPro;

public class SignalTuner : MonoBehaviour
{
    [Header("Frequency Settings")]
    public float currentFrequency = 0f;
    public float frequencyStep = 10f;
    public float frequencyAcceleration = 20f;
    public float maxFrequencyStep = 50f;
    public float minFrequency = 0f;
    public float maxFrequency = 200f;
    private float incrHoldTimer = 0f;
    private float decrHoldTimer = 0f;

    [Header("Interaction Settings")]
    public bool isInteracting = false;
    public KeyCode interactionKey = KeyCode.F;
    public KeyCode increaseKey = KeyCode.E;
    public KeyCode decreaseKey = KeyCode.Q;

    [Header("UI Elements")]
    public TMP_Text frequencyText;

    [Header("Positions")]
    public Vector3 idlePosition = new Vector3(0.3f, -0.3f, 0.5f);
    public Vector3 focusPosition = new Vector3(0f, -0.1f, 0.3f);
    public float focusSmoothSpeed = 5f;

    [Header("Rotation Settings")]
    public float idleYRotation = -21f;
    public float focusYRotation = 3f;
    public float rotationSmoothSpeed = 5f;
    public bool useSmoothRotation = true;

    [Header("Audio Settings")]
    public AudioSource tuningSource;       // default tuning noise
    public AudioSource sfxSource;          // click adjust noise
    public AudioSource glitchSource;
    public AudioSource pureSource;
    public AudioSource beepSource;
    public float crossfadeRange = 20f;
    public AudioClip tuningClip;
    public AudioClip clickClip;
    public AudioClip glitchClip;
    public AudioClip pureClip;
    public AudioClip beepClip;
    public float lowVolume = 0.2f;
    public float highVolume = 0.6f;
    public float doorBaselineVolume = 0.2f;   // the fixed volume for the door's low tone
    public float fullUnlockToneVolume = 1.0f;   // the maximum volume for the unlock tone overlay
    public float beepRange = 10f;             // Range (in frequency units) for the beep to become active.
    public float fullBeepVolume = 0.8f;         // Maximum volume for the beep tone.

    // New fields for crossfading
    private bool overrideGoodSignal = false;
    // New public field to store the door's unlock frequency.
    public float targetUnlockFrequency = -1f;

    private bool lastOverrideState = false;
    private float lastTargetFrequency = -1f;

    // Updated SetGoodSignalActive method
    public void SetGoodSignalActive(bool active, float doorFrequency = -1f)
    {
        // Only log/act if there's an actual change:
        if (overrideGoodSignal != active || (active && doorFrequency != targetUnlockFrequency))
        {
            overrideGoodSignal = active;
            if (active)
            {
                targetUnlockFrequency = doorFrequency;
                Debug.Log("Setting target unlock frequency to " + doorFrequency);
            }
            else
            {
                targetUnlockFrequency = -1f;
                Debug.Log("Clearing target unlock frequency");
            }

            // Store the new values so we know the last state.
            lastOverrideState = overrideGoodSignal;
            lastTargetFrequency = targetUnlockFrequency;
        }
    }

    private void Start()
    {
        transform.localPosition = idlePosition;
        if (!useSmoothRotation)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, idleYRotation, transform.localRotation.eulerAngles.z);
        }

        // Regular tuning sound.
        if (tuningSource != null)
        {
            tuningSource.clip = tuningClip;
            tuningSource.loop = true;
            tuningSource.volume = lowVolume;
            tuningSource.Play();
        }

        // Initialize glitchSource.
        if (glitchSource != null)
        {
            glitchSource.clip = glitchClip;
            glitchSource.loop = true;
            glitchSource.volume = 0f;  // Start muted.
            glitchSource.Play();
        }

        // Initialize pureSource.
        if (pureSource != null)
        {
            pureSource.clip = pureClip;
            pureSource.loop = true;
            pureSource.volume = 0f;
            // Option: Play it now so volume adjustments work smoothly.
            pureSource.Play();
        }

        // Initialize beepSource.
        if (beepSource != null)
        {
            beepSource.clip = beepClip;
            beepSource.loop = true;
            beepSource.volume = 0f;
            // Option: Start it now.
            beepSource.Play();
        }
    }


    private void Update()
    {
        // Toggle interaction state on F.
        if (Input.GetKeyDown(interactionKey))
        {
            isInteracting = !isInteracting;
            if (!useSmoothRotation)
            {
                if (isInteracting)
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, focusYRotation, transform.localRotation.eulerAngles.z);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, idleYRotation, transform.localRotation.eulerAngles.z);
                }
            }
        }

        // Frequency adjustment (continuous input with acceleration)
        if (isInteracting)
        {
            if (Input.GetKey(increaseKey))
            {
                incrHoldTimer += Time.deltaTime;
                float currentStep = frequencyStep + frequencyAcceleration * incrHoldTimer;
                currentStep = Mathf.Clamp(currentStep, frequencyStep, maxFrequencyStep);
                currentFrequency += currentStep * Time.deltaTime;
                if (currentFrequency > maxFrequency) currentFrequency = minFrequency;
                if (Input.GetKeyDown(increaseKey) && sfxSource != null && clickClip != null)
                {
                    sfxSource.PlayOneShot(clickClip);
                }
            }
            else { incrHoldTimer = 0f; }

            if (Input.GetKey(decreaseKey))
            {
                decrHoldTimer += Time.deltaTime;
                float currentStep = frequencyStep + frequencyAcceleration * decrHoldTimer;
                currentStep = Mathf.Clamp(currentStep, frequencyStep, maxFrequencyStep);
                currentFrequency -= currentStep * Time.deltaTime;
                if (currentFrequency < minFrequency) currentFrequency = maxFrequency;
                if (Input.GetKeyDown(decreaseKey) && sfxSource != null && clickClip != null)
                {
                    sfxSource.PlayOneShot(clickClip);
                }
            }
            else { decrHoldTimer = 0f; }
        }

        // Snap frequency if close enough.
        if (!Input.GetKey(increaseKey) && !Input.GetKey(decreaseKey))
        {
            float snapThreshold = 0.1f;
            float rounded = Mathf.Round(currentFrequency);
            if (Mathf.Abs(currentFrequency - rounded) < snapThreshold)
                currentFrequency = rounded;
        }

        // Update UI text.
        if (frequencyText != null)
        {
            frequencyText.text = "kHz: " + currentFrequency.ToString("F2");
        }

        // Lerp position and rotation.
        Vector3 targetPos = isInteracting ? focusPosition : idlePosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * focusSmoothSpeed);
        if (useSmoothRotation)
        {
            float targetY = isInteracting ? focusYRotation : idleYRotation;
            Quaternion targetRot = Quaternion.Euler(transform.localRotation.eulerAngles.x, targetY, transform.localRotation.eulerAngles.z);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, Time.deltaTime * rotationSmoothSpeed);
        }

        // 6) Audio adjustments.
        if (overrideGoodSignal && targetUnlockFrequency >= 0)
        {
            // Mute the normal tuning audio.
            if (tuningSource != null)
                tuningSource.volume = 0f;

            // Compute how far we are from the target frequency.
            float diff = Mathf.Abs(currentFrequency - targetUnlockFrequency);
            // Compute matchFactor: when diff == 0, matchFactor will be 1; if diff >= crossfadeRange, matchFactor will be 0.
            float matchFactor = Mathf.Clamp01(1f - (diff / crossfadeRange));

            // Set the glitchSource volume to fade out as we get closer.
            if (glitchSource != null)
                glitchSource.volume = doorBaselineVolume * (1f - matchFactor);
            // Set the pureSource volume to fade in as we get closer.
            if (pureSource != null)
                pureSource.volume = matchFactor * fullUnlockToneVolume;

            // Additionally, compute the beep factor based on a much tighter range.
            float beepFactor = 0f;
            if (diff < beepRange)
            {
                beepFactor = Mathf.Clamp01(1f - (diff / beepRange));
            }
            if (beepSource != null)
                beepSource.volume = beepFactor * fullBeepVolume;
        }
        else
        {
            // When not in door mode, stop door-specific sounds and revert to normal tuning.
            if (glitchSource != null)
                glitchSource.volume = 0f;
            if (pureSource != null)
                pureSource.volume = 0f;
            if (beepSource != null)
                beepSource.volume = 0f;
            if (tuningSource != null)
                tuningSource.volume = isInteracting ? highVolume : lowVolume;
        }
    }
}
