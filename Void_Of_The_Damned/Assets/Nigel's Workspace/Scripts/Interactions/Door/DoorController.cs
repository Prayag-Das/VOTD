using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public enum MoveDirection { Up, Down, Left, Right }

    [Header("Door Movement Settings")]
    [SerializeField] private MoveDirection direction = MoveDirection.Up;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float openDelay = 0f;
    [SerializeField] private float closeDelay = 3f;
    [SerializeField] private bool autoClose = true;

    // SFX
    [Header("Audio Settings")]
    [SerializeField] private AudioSource doorSource;     // assign your slide AudioSource here
    [SerializeField] private AudioClip slideClip;      // your “door slide” sound
    [Tooltip("Seconds to wait after openDelay before playing slide SFX")]
    [SerializeField] private float slideDelay = 0.1f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = GetTargetPosition();
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 directionVector = direction switch
        {
            MoveDirection.Up => Vector3.up,
            MoveDirection.Down => Vector3.down,
            MoveDirection.Left => Vector3.left,
            MoveDirection.Right => Vector3.right,
            _ => Vector3.up,
        };

        return initialPosition + directionVector * moveDistance;
    }

    public void ActivateDoor()
    {
        if (!isMoving)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        isMoving = true;

        yield return new WaitForSeconds(openDelay);

        // Slide SFX
        if (doorSource != null && slideClip != null)
        {
            yield return new WaitForSeconds(slideDelay);
            doorSource.PlayOneShot(slideClip);
        }


        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (autoClose)
        {
            yield return new WaitForSeconds(closeDelay);
            StartCoroutine(CloseDoor());
        }
        else
        {
            isMoving = false;
        }
    }

    private IEnumerator CloseDoor()
    {
        while (Vector3.Distance(transform.position, initialPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}
