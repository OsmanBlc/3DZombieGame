using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;
    public float airControlMultiplier = 0.5f;

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

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private float xRotation = 0f;
    private bool isGrounded;
    private bool isCrouching;
    private float currentSpeed;
    private float normalHeight;
    private Vector3 normalCenter;

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