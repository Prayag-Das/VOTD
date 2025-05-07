using UnityEngine;

public class PDAController : MonoBehaviour
{
    [Header("PDA Components")]
    [SerializeField] private GameObject pdaPanel;        // Panel to show/hide
    [SerializeField] private PDAInventoryDisplay pdaUI;           // Script to refresh item list

    [Header("Player Controls Handler")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCameraController cameraController;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource clickSource;    // one-shot click SFX
    [SerializeField] private AudioClip clickClip;      // click sound

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool isActive = !pdaPanel.activeSelf;

            // Click SFX
            if (clickSource != null && clickClip != null)
                clickSource.PlayOneShot(clickClip);

            pdaPanel.SetActive(isActive);

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;

            playerController.SetMovementEnabled(!isActive);
            cameraController.SetCameraLookEnabled(!isActive);

            // refresh UI
            if (isActive)
                pdaUI.RefreshUI();

            // hum vs background music
            if (GameAudioManager.Instance != null)
            {
                if (isActive)
                {
                    GameAudioManager.Instance.PauseMusic();
                    GameAudioManager.Instance.PlayHum();
                }
                else
                {
                    GameAudioManager.Instance.StopHum();
                    GameAudioManager.Instance.ResumeMusic();
                }
            }
        }
    }
}