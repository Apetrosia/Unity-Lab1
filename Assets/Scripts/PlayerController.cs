using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки")]
    public InputActionAsset inputActions;
    public Transform cameraTransform;
    public float moveSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private bool wantsToJump;
    private bool isGrounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");

        if (moveAction == null || jumpAction == null)
        {
            Debug.LogError("Не найдены действия Move или Jump!");
        }
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
    }

    void Update()
    {
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            wantsToJump = true;
        }
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

        Vector3 movement = cameraForward * moveInput.y + cameraRight * moveInput.x;

        rb.AddForce(movement * moveSpeed, ForceMode.Acceleration);

        if (wantsToJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            wantsToJump = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
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