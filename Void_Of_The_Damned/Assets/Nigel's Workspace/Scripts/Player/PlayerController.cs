using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Push Settings")]
    [SerializeField] private float pushStrength = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClipA;
    [SerializeField] private AudioClip footstepClipB;
    [SerializeField] private AudioClip jumpClip;

    [Tooltip("Seconds between footsteps at walk speed")]
    [SerializeField] private float baseFootstepInterval = 0.45f;
    [Tooltip("± pitch variation for footsteps")]
    [SerializeField] private float pitchJitter = 0.05f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private bool isGrounded;
    private bool wasGroundedLastFrame = true;
    private bool canMove = true;
    private bool sprintAllowed = true;

    // Footstep tracking
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

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = (Input.GetKey(KeyCode.LeftShift) && sprintAllowed) ? sprintSpeed : moveSpeed;

        controller.Move(move * speed * Time.deltaTime);

        bool isMoving = moveX != 0 || moveZ != 0;
        animator.SetBool("startwalk", isMoving);
        animator.SetBool("startrun", isMoving && Input.GetKey(KeyCode.LeftShift));

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && !wasGroundedLastFrame)
        {
            animator.SetBool("jump", false);
            animator.SetTrigger("jumpfall");
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            animator.SetTrigger("jump");

            if (audioSource != null && jumpClip != null)
                audioSource.PlayOneShot(jumpClip);
        }

        wasGroundedLastFrame = isGrounded;
    }

    private void HandleFootsteps()
    {
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f;
        float horizSpeed = delta.magnitude / Time.deltaTime;

        bool moving = horizSpeed > 0.1f;
        bool grounded = controller.isGrounded;

        if (grounded && moving)
        {
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
            footstepTimer = 0f;
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
            float pitch = Random.Range(1f - pitchJitter, 1f + pitchJitter);
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null)
        {
            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
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
