using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    //[SerializeField] private float jumpHeight = 2f; dont forget to delete this nigel

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Push Settings")]
    [SerializeField] private float pushStrength = 0.1f; // Controls how strong the push feels

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    // Checks if player is grounded.
    private bool isGrounded;
    private bool wasGroundedLastFrame = true;

    // Checks if player is able to move.
    private bool canMove = true;
    private bool sprintAllowed = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!canMove) return;

        HandleMovement();
        HandleJump();
    }

    // PLAYER MOVEMENT
    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = (Input.GetKey(KeyCode.LeftShift) && sprintAllowed) ? sprintSpeed : moveSpeed;

        controller.Move(move * speed * Time.deltaTime);

        // Update Animator Parameters
        bool isMoving = moveX != 0 || moveZ != 0;
        animator.SetBool("startwalk", isMoving);
        animator.SetBool("startrun", isMoving && Input.GetKey(KeyCode.LeftShift));

        // Clamp downward velocity when grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }

    // JUMP MECHANIC
    private void HandleJump()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && !wasGroundedLastFrame)
        {
            // Just landed
            animator.SetBool("jump", false); // Reset jump on landing
            animator.SetTrigger("jumpfall");
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce; // More responsive jump with a stronger initial force
            animator.SetTrigger("jump");
        }

        wasGroundedLastFrame = isGrounded;

        
    }

    // PUSH MECHANIC
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null) // Ensure the object has a Rigidbody
        {
            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
        }
    }

    public void SetSprintAllowed(bool allowed)
    {
        sprintAllowed = allowed;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetSprintSpeed(float newSprintSpeed)
    {
        sprintSpeed = newSprintSpeed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetSprintSpeed()
    {
        return sprintSpeed;
    }

}
