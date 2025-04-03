using UnityEngine;
using System.Collections;

public class FrameRateController : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [SerializeField] private int targetFrameRate = 60; // Desired frame rate

    [Header("VSync Settings")]
    [SerializeField] private int vSyncCount = 0; // 0 = VSync off, 1 = VSync on

    private void Awake()
    {
        // Apply the settings
        QualitySettings.vSyncCount = vSyncCount;  // Disable VSync if desired
        Application.targetFrameRate = targetFrameRate;

        // Confirm the setting
        Debug.Log($"Target Frame Rate set to: {targetFrameRate} FPS (VSync: {vSyncCount})");
    }

    // Optional: Change frame rate dynamically
    public void SetFrameRate(int newFrameRate)
    {
        Application.targetFrameRate = newFrameRate;
        Debug.Log($"Frame Rate changed to: {newFrameRate} FPS");
    }
}
