using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private Transform holdPoint;       // Position where held objects stay.
    [SerializeField] private float pickupRange = 3f;     // Max distance for picking up objects.
    [SerializeField] private float holdDistance = 2f;    // Fixed distance for held objects.
    [SerializeField] private float smoothSpeed = 15f;    // Smoothing for held object movement.
    [SerializeField] private float throwForce = 3f;     // Throw strength.
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Pickup key.

    private Rigidbody heldObject;
    private Vector3 currentVelocity; // For smooth damping.

    private void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        if (heldObject != null)
        {
            MoveHeldObject();

            // Throw object with Left Mouse Button.
            if (Input.GetMouseButtonDown(0))
            {
                ThrowObject(); // Throw mechanic.
            }
        }
    }

    // Try to pick up an object.
    private void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.GetComponent<Rigidbody>();

                if (heldObject != null)
                {
                    heldObject.useGravity = false;
                    heldObject.linearDamping = 10f;   // Stabilize movement.
                }
            }
        }
    }

    // Move the held object to the fixed distance in front of the player.
    private void MoveHeldObject()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
        heldObject.linearVelocity = Vector3.zero; // Supposedly prevents shaky physics.
        heldObject.MovePosition(Vector3.SmoothDamp(heldObject.position, targetPosition, ref currentVelocity, Time.deltaTime * smoothSpeed));

        // Rotate the object to face forward (optional for stability).
        heldObject.rotation = Quaternion.Lerp(heldObject.rotation, holdPoint.rotation, Time.deltaTime * smoothSpeed);
    }

    // Drop the object without force.
    private void DropObject()
    {
        heldObject.useGravity = true;
        heldObject.linearDamping = 1f;  // Restore default drag.
        heldObject = null;
    }

    // Throw the object with force.
    private void ThrowObject()
    {
        heldObject.useGravity = true;
        heldObject.linearDamping = 1f;  // Restore default drag.
        heldObject.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        heldObject = null;
    }
}
