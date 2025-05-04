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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // attach one AudioSource
    [SerializeField] private AudioClip footstepClipA; // your first footstep
    [SerializeField] private AudioClip footstepClipB; // your second footstep
    [SerializeField] private AudioClip jumpClip;      // jump sound

    [Tooltip("Seconds between footsteps at walk speed")]
    [SerializeField] private float baseFootstepInterval = 0.45f;
    [Tooltip("± pitch variation for footsteps")]
    [SerializeField] private float pitchJitter = 0.05f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    // Checks if player is grounded.
    private bool isGrounded;
    private bool wasGroundedLastFrame = true;

    // Checks if player is able to move.
    private bool canMove = true;
    private bool sprintAllowed = true;

    // Footstep internals
    private Vector3 lastPosition;
    private float footstepTimer = 0f;
    private bool useFirstFoot = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponentInChildren<Animator>();

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (!canMove) return;

        HandleMovement();
        HandleJump();
        HandleFootsteps();
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

    private void HandleFootsteps()
    {
        // Compute horizontal speed from frame-to-frame position delta
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f;
        float horizSpeed = delta.magnitude / Time.deltaTime;

        bool moving = horizSpeed > 0.1f;
        bool grounded = controller.isGrounded;

        if (grounded && moving)
        {
            // Faster for sprint
            float currentBase = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
            float interval = baseFootstepInterval * (moveSpeed / currentBase);

            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = interval;
            }
        }
        else
        {
            footstepTimer = 0f; // reset so first step always plays
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        if (audioSource == null) return;

        AudioClip clip = useFirstFoot ? footstepClipA : footstepClipB;
        useFirstFoot = !useFirstFoot;

        if (clip != null)
        {
            // slight pitch variation
            float pitch = Random.Range(1f - pitchJitter, 1f + pitchJitter);
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip);
        }
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

            // Play jump sound
            if (audioSource != null && jumpClip != null)
                audioSource.PlayOneShot(jumpClip);
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
