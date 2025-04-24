using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //--------------------------------
    // Movement
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Push Settings")]
    [SerializeField] private float pushStrength = 0.1f;   // Rigidbody push impulse

    // --------------------------------
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;      // One AudioSource is fine
    [SerializeField] private AudioClip footstepClipA;    // First foot-step clip
    [SerializeField] private AudioClip footstepClipB;    // Second foot-step clip
    [SerializeField] private AudioClip jumpClip;         // Jump clip

    [Tooltip("Base interval (seconds) between footsteps at walk speed")]
    [SerializeField] private float baseFootstepInterval = 0.45f;

    [Tooltip("How much the foot-step pitch can vary ± (e.g. 0.1 = ±10 %)")]
    [SerializeField] private float pitchJitter = 0.05f;

    //--------------------------------
    // Internals
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canMove = true;
    private Vector3 lastPosition;

    // Foot-step helpers
    private float footstepTimer = 0f;
    private bool useFirstFoot = true;   // alternates A / B

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!canMove) return;

        HandleMovement();
        HandleJump();
        HandleFootsteps();
    }

    //--------------------------------
    // Movement
    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Jump
    private void HandleJump()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;

            // Play jump sound
            if (audioSource && jumpClip)
            {
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(jumpClip);
            }
        }
    }

    // Foot-steps
    private void HandleFootsteps()
    {
        // Calculate horizontal (XZ) velocity manually to ignore the gravity Y component.
        Vector3 horizontalMove = transform.position - lastPosition;
        horizontalMove.y = 0f;                // ignore vertical
        float horizSpeed = horizontalMove.magnitude / Time.deltaTime;
        lastPosition = transform.position;

        bool moving = horizSpeed > 0.1f;      // simple threshold for movement
        bool grounded = controller.isGrounded;  // cached in HandleJump too

        if (grounded && moving)
        {
            // Determine desired footstep interval based on walk vs sprint.
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            float stepInterval = baseFootstepInterval * (walkSpeed / currentSpeed);

            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = stepInterval;
            }
        }
        else
        {
            // Reset timer when standing still or airborne so first step isn't clipped.
            footstepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (audioSource == null) return;

        // Choose clip (alternate A / B)
        AudioClip clip = useFirstFoot ? footstepClipA : footstepClipB;
        useFirstFoot = !useFirstFoot;

        if (clip)
        {
            // Slight random pitch variation
            float rand = Random.Range(1f - pitchJitter, 1f + pitchJitter);
            audioSource.pitch = rand;
            audioSource.PlayOneShot(clip);
        }
    }

    // Enable / disable external movement (e.g. when menus open)
    public void SetMovementEnabled(bool enabled) => canMove = enabled;

    // Rigidbody push
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDir * pushStrength, ForceMode.Impulse);
        }
    }
}
