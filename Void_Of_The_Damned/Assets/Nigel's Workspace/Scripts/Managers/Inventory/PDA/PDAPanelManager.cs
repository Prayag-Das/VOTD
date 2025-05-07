using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDAPanelManager : MonoBehaviour
{
    [Header("PDA Panels")]
    public GameObject tasksPanel;
    public GameObject itemsPanel;
    public GameObject logsPanel;
    public GameObject PDA;

    [Header("Player References")]
    public PlayerController playerController;
    public PlayerCameraController cameraController;

    public void OpenTasksPanel()
    {
        tasksPanel.SetActive(true);
        itemsPanel.SetActive(false);
        logsPanel.SetActive(false);
    }

    public void OpenItemsPanel()
    {
        tasksPanel.SetActive(false);
        itemsPanel.SetActive(true);
        logsPanel.SetActive(false);
    }

    public void OpenLogsPanel()
    {
        tasksPanel.SetActive(false);
        itemsPanel.SetActive(false);
        logsPanel.SetActive(true);
    }

    public void ClosePDA()
    {
        PDA.SetActive(false);

        // Re-enable player movement & camera
        if (playerController != null)
            playerController.SetMovementEnabled(true);

        if (cameraController != null)
            cameraController.SetCameraLookEnabled(true);

        // Restore cursor lock state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}