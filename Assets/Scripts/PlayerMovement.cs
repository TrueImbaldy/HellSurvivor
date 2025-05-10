using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.91f;

    //This is a reference to the main camera's transform, this is so movement is relative to where the camera is facing
    public Transform cameraTransform;
    
    private CharacterController characterController;
    //velocity stores the current vertical motion such as falling or jumping
    private Vector3 velocity;
    private bool isGrounded;
    
    void Start() //runs once at the beginning
    {
        //gets and stores the CharacterController so we can move the character
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Ground check
        isGrounded = characterController.isGrounded;
        //if they are grounded and they're falling then it slightly pushes them down to keep them grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        characterController.Move(move * walkSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        
        characterController.Move(velocity * Time.deltaTime);
    }
}
