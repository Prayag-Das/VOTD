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
    private Animator animator;                
    private Vector3 velocity;
    private bool isGrounded;
    private bool canMove = true;
    private Vector3 lastMovePosition;
    private Vector3 lastStepPosition;

    // Foot-step helpers
    private float footstepTimer = 0f;
    private bool useFirstFoot = true;   // alternates A / B

    // Awake / Start
    private void Awake()                      
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();   
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

        Vector3 direction = transform.right * moveX + transform.forward * moveZ;
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // 1) Move the character
        controller.Move(direction * moveSpeed * Time.deltaTime);

        // 2) Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 3) Calculate actual horizontal speed from position delta
        Vector3 horizontalDelta = transform.position - lastMovePosition;
        horizontalDelta.y = 0f;
        float actualSpeed = horizontalDelta.magnitude / Time.deltaTime;
        lastMovePosition = transform.position;

        // 4) Drive the Animator "Speed" parameter
        if (animator != null)
        {
            float normalized = Mathf.InverseLerp(0f, sprintSpeed, actualSpeed);
            animator.SetFloat("Speed", normalized, 0.1f, Time.deltaTime);
            Debug.Log($"Normalized Speed: {normalized:F2}");
        }
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
        Vector3 deltaStep = transform.position - lastStepPosition;
        float horizSpeed = new Vector3(deltaStep.x, 0, deltaStep.z).magnitude / Time.deltaTime;

        if (controller.isGrounded && horizSpeed > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstep();
                footstepTimer = baseFootstepInterval * (walkSpeed / (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed));
            }
        }
        else
        {
            footstepTimer = 0;
        }

        lastStepPosition = transform.position;
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
