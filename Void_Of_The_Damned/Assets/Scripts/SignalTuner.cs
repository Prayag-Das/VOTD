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

    // Door-mode state
    private bool overrideGoodSignal = false;
    public float targetUnlockFrequency = -1f;

    // Debug helper (optional)
    private bool lastInteractingState = false;

    // Called by doors
    public void SetGoodSignalActive(bool active, float doorFrequency = -1f)
    {
        if (overrideGoodSignal != active || (active && doorFrequency != targetUnlockFrequency))
        {
            overrideGoodSignal = active;
            targetUnlockFrequency = active ? doorFrequency : -1f;
            Debug.Log(active
                ? $"[Tuner] Door mode ON: target={doorFrequency}"
                : "[Tuner] Door mode OFF");
        }
    }

    private void Start()
    {
        // Initialize all audio sources:
        if (tuningSource != null)
        {
            tuningSource.clip = tuningClip;
            tuningSource.loop = true;
            tuningSource.volume = lowVolume;
            tuningSource.Play();
        }

        if (glitchSource != null)
        {
            glitchSource.clip = glitchClip;
            glitchSource.loop = true;
            glitchSource.volume = 0f;
            glitchSource.Play();
        }

        if (pureSource != null)
        {
            pureSource.clip = pureClip;
            pureSource.loop = true;
            pureSource.volume = 0f;
            pureSource.Play();
        }

        if (beepSource != null)
        {
            beepSource.clip = beepClip;
            beepSource.loop = true;
            beepSource.volume = 0f;
            beepSource.Play();
        }
    }

    private void Update()
    {
        // 0) Toggle interaction state on F each frame
        if (Input.GetKeyDown(interactionKey))
        {
            isInteracting = !isInteracting;
            Debug.Log($"[Tuner] isInteracting = {isInteracting}");
        }

        // 1) Frequency adjustment (continuous input with acceleration)
        if (isInteracting)
        {
            // Increase
            if (Input.GetKey(increaseKey))
            {
                incrHoldTimer += Time.deltaTime;
                float step = Mathf.Clamp(frequencyStep + frequencyAcceleration * incrHoldTimer,
                                         frequencyStep, maxFrequencyStep);
                currentFrequency += step * Time.deltaTime;
                if (currentFrequency > maxFrequency) currentFrequency = minFrequency;
                if (Input.GetKeyDown(increaseKey) && sfxSource != null && clickClip != null)
                    sfxSource.PlayOneShot(clickClip);
            }
            else incrHoldTimer = 0f;

            // Decrease
            if (Input.GetKey(decreaseKey))
            {
                decrHoldTimer += Time.deltaTime;
                float step = Mathf.Clamp(frequencyStep + frequencyAcceleration * decrHoldTimer,
                                         frequencyStep, maxFrequencyStep);
                currentFrequency -= step * Time.deltaTime;
                if (currentFrequency < minFrequency) currentFrequency = maxFrequency;
                if (Input.GetKeyDown(decreaseKey) && sfxSource != null && clickClip != null)
                    sfxSource.PlayOneShot(clickClip);
            }
            else decrHoldTimer = 0f;
        }

        // 2) Snap frequency when keys released
        if (!Input.GetKey(increaseKey) && !Input.GetKey(decreaseKey))
        {
            float rounded = Mathf.Round(currentFrequency);
            if (Mathf.Abs(currentFrequency - rounded) < 0.1f)
                currentFrequency = rounded;
        }

        // 3) Update UI
        if (frequencyText != null)
            frequencyText.text = $"kHz: {currentFrequency:F2}";

        // 4) Audio adjustments & door crossfade
        if (overrideGoodSignal && targetUnlockFrequency >= 0f)
        {
            if (tuningSource != null) tuningSource.volume = 0f;

            float diff = Mathf.Abs(currentFrequency - targetUnlockFrequency);
            float match = Mathf.Clamp01(1f - diff / crossfadeRange);

            if (glitchSource != null)
                glitchSource.volume = doorBaselineVolume * (1f - match);
            if (pureSource != null)
                pureSource.volume = match * fullUnlockToneVolume;

            float beep = diff < beepRange
                ? Mathf.Clamp01(1f - diff / beepRange)
                : 0f;
            if (beepSource != null)
                beepSource.volume = beep * fullBeepVolume;
        }
        else
        {
            if (glitchSource != null) glitchSource.volume = 0f;
            if (pureSource != null) pureSource.volume = 0f;
            if (beepSource != null) beepSource.volume = 0f;
            if (tuningSource != null)
                tuningSource.volume = isInteracting ? highVolume : lowVolume;
        }
    }
}
