using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f;
    public float airControlMultiplier = 0.5f;

    [Header("Gravity")]
    public float fallMultiplier = 2.5f; // Düşüş hissini toklaştıran çarpan

    [Header("Mouse Look")]
    public float mouseSensitivity = 150f;
    public Transform cameraTransform;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("Crouch")]
    public float crouchSpeed = 2.5f;
    public float crouchHeight = 1f;

    [Header("Footstep")]
    public AudioClip footstepClip;
    public AudioSource footstepSource;
    public float footstepVolume = 0.55f;
    public float walkStepInterval = 0.45f;
    public float runStepInterval = 0.32f;
    public float crouchStepInterval = 0.6f;
    public float footstepMinSpeed = 0.2f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private float xRotation = 0f;
    private bool isGrounded;
    private bool isCrouching;
    private float currentSpeed;
    private float normalHeight;
    private Vector3 normalCenter;
    private float nextFootstepTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb.freezeRotation = true;

        if (capsuleCollider != null)
        {
            normalHeight = capsuleCollider.height;
            normalCenter = capsuleCollider.center;
        }

        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.playOnAwake = false;
            footstepSource.spatialBlend = 0f;
        }
    }

    void Update()
    {
        GroundCheck();
        LookAround();
        HandleJump();
        HandleCrouch();
    }

    void FixedUpdate()
    {
        MovePlayer();
        HandleFootsteps();
        ApplyExtraGravity(); // Düşüşü hızlandıran fonksiyonu buraya ekledik
    }

    void GroundCheck()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (isCrouching)
            currentSpeed = crouchSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        move = move.normalized;

        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;

        Vector3 velocity = move * currentSpeed * controlMultiplier;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void HandleFootsteps()
    {
        if (footstepClip == null || footstepSource == null || !isGrounded)
            return;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude < footstepMinSpeed)
        {
            nextFootstepTime = 0f;
            return;
        }

        if (Time.time < nextFootstepTime)
            return;

        footstepSource.pitch = Random.Range(0.92f, 1.08f);
        footstepSource.PlayOneShot(footstepClip, footstepVolume * SettingsManager.SfxVolume);

        if (isCrouching)
            nextFootstepTime = Time.time + crouchStepInterval;
        else if (Input.GetKey(KeyCode.LeftShift))
            nextFootstepTime = Time.time + runStepInterval;
        else
            nextFootstepTime = Time.time + walkStepInterval;
    }

    void ApplyExtraGravity()
    {
        // Eğer karakter aşağı doğru düşüyorsa ekstra yerçekimi uygula
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCrouch()
    {
        if (capsuleCollider == null) return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = new Vector3(normalCenter.x, crouchHeight / 2f, normalCenter.z);
        }
        else
        {
            isCrouching = false;
            capsuleCollider.height = normalHeight;
            capsuleCollider.center = normalCenter;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
