using UnityEngine;
using System.Collections;

public class PDAController : MonoBehaviour
{
    [Header("PDA Components")]
    [SerializeField] private GameObject pdaPanel;                // Panel to show/hide
    [SerializeField] private PDAInventoryDisplay pdaUI;          // Script to refresh item list

    [Header("Player Controls Handler")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCameraController cameraController;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;    // Now settable in Inspector

    [Header("Audio Settings")]
    [SerializeField] private AudioSource clickSource;            // one-shot click SFX
    [SerializeField] private AudioClip clickClip;              // click sound
    [SerializeField] private AudioSource humSource;              // looping hum SFX
    [SerializeField] private AudioClip humClip;                // hum sound (loop)
    [SerializeField] private AudioSource backgroundMusicSource;  // the main bg music

    private void Start()
    {
        // Prepare hum audio source but don't play yet
        if (humSource != null && humClip != null)
        {
            humSource.clip = humClip;
            humSource.loop = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool isActive = !pdaPanel.activeSelf;

            // click SFX
            if (clickSource != null && clickClip != null)
                clickSource.PlayOneShot(clickClip);

            pdaPanel.SetActive(isActive);

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;

            playerController.SetMovementEnabled(!isActive);
            cameraController.SetCameraLookEnabled(!isActive);

            if (isActive)
                pdaUI.RefreshUI();

            // SFX; hum vs background music
            if (isActive)
            {
                // Pause background music
                if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
                    backgroundMusicSource.Pause();

                // Start hum loop
                if (humSource != null && !humSource.isPlaying)
                    humSource.Play();
            }
            else
            {
                // Stop hum
                if (humSource != null && humSource.isPlaying)
                    humSource.Stop();

                // Resume background music
                if (backgroundMusicSource != null)
                    backgroundMusicSource.UnPause();
            }
        }
    }
}
