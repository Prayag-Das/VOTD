using UnityEngine;
using TMPro;

public class SignalTuner : MonoBehaviour
{
    [Header("Frequency Settings")]
    public float currentFrequency = 0f;
    public float frequencyStep = 10f;
    public float minFrequency = 0f;
    public float maxFrequency = 200f;

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
    // Set the Y rotation for idle and focused states.
    public float idleYRotation = -21f;
    public float focusYRotation = 3f;
    public float rotationSmoothSpeed = 5f;
    public bool useSmoothRotation = true; // if false, the rotation change will be instantaneous when toggling focus

    [Header("Audio Settings")]
    public AudioSource tuningSource; // default tuning noise
    public AudioSource goodSignalSource; // door unlocking signal
    public AudioSource sfxSource; // click adjust noise

    public AudioClip tuningClip;
    public AudioClip goodSignalClip;
    public AudioClip clickClip;

    // Tuning volumes – lower when idle, higher when interacting.
    public float lowVolume = 0.2f;
    public float highVolume = 0.6f;

    // Override the tuning audio when door frequency match
    private bool overrideGoodSignal = false;

    public void SetGoodSignalActive(bool active)
    {
        overrideGoodSignal = active;
    }

    private void Start()
    {
        // Set initial transform position and rotation
        transform.localPosition = idlePosition;
        if (!useSmoothRotation)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, idleYRotation, transform.localRotation.eulerAngles.z);
        }

        if (tuningSource != null)
        {
            tuningSource.clip = tuningClip;
            tuningSource.loop = true;
            tuningSource.volume = lowVolume;
            tuningSource.Play();
        }

        if (goodSignalSource != null)
        {
            goodSignalSource.clip = goodSignalClip;
            goodSignalSource.loop = true;
            goodSignalSource.volume = 0f;
            // We do not call Play() now; it will be triggered if needed.
        }
    }

    private void Update()
    {
        // 1) Toggle interaction state on F.
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

        // 2) If interacting, allow frequency changes and play click sound when E or Q is pressed.
        if (isInteracting)
        {
            if (Input.GetKeyDown(increaseKey))
            {
                currentFrequency += frequencyStep;
                if (currentFrequency > maxFrequency) currentFrequency = minFrequency;
                if (sfxSource != null && clickClip != null)
                    sfxSource.PlayOneShot(clickClip);

            }

            if (Input.GetKeyDown(decreaseKey))
            {
                currentFrequency -= frequencyStep;
                if (currentFrequency < minFrequency) currentFrequency = maxFrequency;
                if (sfxSource != null && clickClip != null)
                    sfxSource.PlayOneShot(clickClip);

            }
        }

        // 3) Update the UI text.
        if (frequencyText != null)
        {
            frequencyText.text = "kHz: " + currentFrequency.ToString("F2");
        }

        // 4) Smoothly move the tuner between its idle and focused positions.
        Vector3 targetPos = isInteracting ? focusPosition : idlePosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * focusSmoothSpeed);

        // 5) Smoothly rotate (if enabled) between idle and focused Y rotation.
        if (useSmoothRotation)
        {
            float targetY = isInteracting ? focusYRotation : idleYRotation;
            Quaternion targetRot = Quaternion.Euler(transform.localRotation.eulerAngles.x, targetY, transform.localRotation.eulerAngles.z);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, Time.deltaTime * rotationSmoothSpeed);
        }

        // 6) Audio adjustments.
        if (overrideGoodSignal)
        {
            // If override is active, ensure the good signal source is playing.
            if (goodSignalSource != null && !goodSignalSource.isPlaying)
                goodSignalSource.Play();
            if (goodSignalSource != null)
                goodSignalSource.volume = 1.0f;
            if (tuningSource != null)
                tuningSource.volume = 0f;
        }
        else
        {
            // If the good signal override is not active, stop the good signal sound (if playing)
            if (goodSignalSource != null && goodSignalSource.isPlaying)
                goodSignalSource.Stop();
            // Adjust the tuning source's volume based on whether we are interacting.
            if (tuningSource != null)
                tuningSource.volume = isInteracting ? highVolume : lowVolume;
        }
    }
}
