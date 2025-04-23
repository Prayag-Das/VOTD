using UnityEngine;
using System.Collections;

public class PDAController : MonoBehaviour
{
    [Header("PDA Components")]
    [SerializeField] private GameObject pdaPanel; // Panel to show/hide
    [SerializeField] private PDAInventoryDisplay pdaUI; // Script to refresh item list

    [Header("Player Controls Handler")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCameraController cameraController;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab; // Now settable in Inspector

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool isActive = !pdaPanel.activeSelf;
            pdaPanel.SetActive(isActive);

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;

            playerController.SetMovementEnabled(!isActive);
            cameraController.SetCameraLookEnabled(!isActive);

            if (isActive)
            {
                pdaUI.RefreshUI();
            }
        }
    }
}
