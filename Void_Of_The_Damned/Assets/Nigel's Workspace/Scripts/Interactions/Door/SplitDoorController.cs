using UnityEngine;
using System.Collections;

public class SplitDoorController : MonoBehaviour
{
    public enum MoveDirection { LeftRight, UpDown }

    [Header("Door Parts")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Door Movement Settings")]
    [SerializeField] private MoveDirection moveDirection = MoveDirection.LeftRight;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float openDelay = 0f;
    [SerializeField] private float closeDelay = 3f;
    [SerializeField] private bool autoClose = true;

    private Vector3 leftInitialPos;
    private Vector3 rightInitialPos;
    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;

    private bool isMoving = false;

    private void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("SplitDoorController: Left or Right door is not assigned!");
            return;
        }

        leftInitialPos = leftDoor.position;
        rightInitialPos = rightDoor.position;

        Vector3 localOffset = moveDirection == MoveDirection.LeftRight ? transform.right : transform.up;
        leftTargetPos = leftInitialPos - localOffset * moveDistance;   // move left door "out"
        rightTargetPos = rightInitialPos + localOffset * moveDistance; // move right door "out"
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

        while (Vector3.Distance(leftDoor.position, leftTargetPos) > 0.01f ||
               Vector3.Distance(rightDoor.position, rightTargetPos) > 0.01f)
        {
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftTargetPos, moveSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightTargetPos, moveSpeed * Time.deltaTime);
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
        while (Vector3.Distance(leftDoor.position, leftInitialPos) > 0.01f ||
               Vector3.Distance(rightDoor.position, rightInitialPos) > 0.01f)
        {
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftInitialPos, moveSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightInitialPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}