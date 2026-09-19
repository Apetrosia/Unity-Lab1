using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки")]
    public InputActionAsset inputActions;
    public Transform cameraTransform;
    public float moveSpeed = 10f;
    public float jumpForce = 5f;

    [Header("Настройки респавна")]
    public float fallLimit = -5f;
    public Vector3 respawnPoint = new Vector3(0, 0.5f, 0);

    [Header("Настройки стены")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 8f;
    public float wallMass = 0.05f;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction wallHoldAction;

    private bool wantsToJump;
    private bool isGrounded = false;
    private bool isWallStuck = false;
    private Vector3 wallNormal;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");
        wallHoldAction = inputActions.FindAction("WallHold");
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (wallHoldAction != null) wallHoldAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (wallHoldAction != null) wallHoldAction.Disable();
    }

    void Update()
    {
        if (transform.position.y < fallLimit)
        {
            Respawn();
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded && !isWallStuck)
        {
            wantsToJump = true;
        }

        if (isWallStuck && jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            isWallStuck = false;
            rb.mass = 1f;
            Vector3 jumpDirection = (Vector3.up * 2 + wallNormal).normalized;
            rb.AddForce(jumpDirection * wallJumpForce, ForceMode.Impulse);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isGrounded = false;
        isWallStuck = false;
        rb.mass = 1f;
    }

    void FixedUpdate()
    {
        if (moveAction == null) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        if (isWallStuck)
        {
            Vector3 desiredMoveDir = cameraForward * moveInput.y + cameraRight * moveInput.x;
            Vector3 wallMoveDir = Vector3.ProjectOnPlane(desiredMoveDir, wallNormal).normalized;
            rb.AddForce(wallMoveDir * moveSpeed, ForceMode.Acceleration);

            bool isHoldingWall = wallHoldAction != null && wallHoldAction.IsPressed();

            if (isHoldingWall)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
        }
        else
        {
            Vector3 movement = cameraForward * moveInput.y + cameraRight * moveInput.x;
            rb.AddForce(movement * moveSpeed, ForceMode.Acceleration);

            if (wantsToJump)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                wantsToJump = false;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
            isWallStuck = false;
            rb.mass = 1f;
        }

        if (!isGrounded)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.y) < 0.7f)
                {
                    isWallStuck = true;
                    wallNormal = contact.normal;
                    rb.mass = wallMass;
                    break;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = false;
        }
    }
}