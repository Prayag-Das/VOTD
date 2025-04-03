using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 2f;
    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = 0f; // Ensures camera starts level
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void Update()
    {
        HandleCamera();
    }

    // CAMERA CONTROL
    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Vertical rotation (pitch) — tilts the camera up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (yaw) — rotates the **Player Body**
        playerBody.Rotate(Vector3.up * mouseX);
    }
}