using UnityEngine;
using System.Collections;

public class StarDestroyerMover : MonoBehaviour
{
    public float normalSpeed = 20f;
    public float fastSpeed = 3000f;

    private float currentSpeed;
    private bool isBoosting = false;
    private bool hasTeleported = false;

    void Start()
    {
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        // Move along the Z axis
        transform.position += new Vector3(0f, 0f, -currentSpeed * Time.deltaTime);

        // Trigger boost once when crossing -2
        if (!isBoosting && transform.position.z <= -2f && !hasTeleported)
        {
            StartCoroutine(BoostAndTeleport());
        }

        // After teleport, slow down when reaching z = 260
        if (hasTeleported && transform.position.z <= 260f)
        {
            currentSpeed = normalSpeed;

            // Reset flags for the next loop
            isBoosting = false;
            hasTeleported = false;
        }
    }

    IEnumerator BoostAndTeleport()
    {
        isBoosting = true;
        currentSpeed = fastSpeed;

        yield return new WaitForSeconds(2f);

        transform.position = new Vector3(transform.position.x, transform.position.y, 2000f);
        hasTeleported = true;
    }
}
