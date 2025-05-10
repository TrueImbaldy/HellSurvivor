using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public Transform cameraTransform; //Reference to the camera
    public float mouseSensitivity = 100f;
    public float xClamp = 85f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private bool isGrounded;

    private float xRotation = 0f;
    private InputAction escapeAction; //Reference to the escape action

    //called when script first initialises
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        
        //Initialise Escape action
        escapeAction = controls.Player.Escape;
        
    }

    public GameObject crosshairUI;

    //called just before the first frame update
    private void Start()
    {
        //Lock the cursor to the center and hide it at the start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(true);
        }
    }

    private void OnEnable()
    {
        controls.Enable();
        escapeAction.performed += ctx => UnlockCursor();
    }

    private void OnDisable()
    {
        controls.Disable();
        escapeAction.performed -= ctx => UnlockCursor();
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        
        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (crosshairUI != null)
                crosshairUI.SetActive(true);
        }
        
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(false);
        }
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * walkSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
}
