using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

// add components needed for script to work
// prevents compile errors
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController Instance;

    public bool inMenu;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed;
    public float sprintMultiplier;

    [Header("Crouch")]
    private Vector3 centre = new (0, 0, 0);
    private float height = 2f;
    private Vector3 crouchCentre = new (0, -0.25f, 0);
    private float crouchHeight = 1.5f;

    public bool isCrouching;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Wall Jump")]
    public bool canWallJump;
    [SerializeField] private float snapKickJumpForce;
    [SerializeField] private float sideKickJumpForce;
    public bool snapKickJump;
    public bool sideKickJump;
    private int wallJumpCounter;

    [Header("Side Kick")]
    public bool sideKick;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    public CharacterController characterController;
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CapsuleCollider col;

    [Header("Animators")]
    [SerializeField] private Animator cameraAnimator;

    private Vector3 currentMovement;
    private float verticalRotation;

    // directly manipulate the variable
    public float CurrentSpeed;

    private void Awake()
    {
        CheckReferences();

        inMenu = false;

        Instance = this;
    }

    private void CheckReferences()
    {
        // avoid anything missing that could break the game
        // a really awful way to do this but i wanted to learn how to use returns
        // bite me
        bool allReferencesGrabbed = CheckNull();

        if (!allReferencesGrabbed)
        {
            characterController = GetComponent<CharacterController>();
            mainCamera = GetComponentInChildren<CinemachineCamera>();
            col = GetComponent<CapsuleCollider>();
        }
    }

    bool CheckNull()
    {
        // variables may be set as null/are missing
        // these are failsafes incase they do
        if (characterController || mainCamera || col == null)
        {
            return false;
        }
        return true;
    }

    void Update()
    {
        SetCursor();

        if (!inMenu)
        {
            HandleRotation();
        }
    }

    private void FixedUpdate()
    {
        SpeedMultiplierHandler();
        HandleMovement();
        HandleCrouch();
        WallJump();
    }

    void SetCursor()
    {
        if (inMenu)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private Vector3 CalculateWorldDirection()
    {
        // i am using the players input to find a local position in x and y
        // then using that local position to then find the global space position which can then be fed back to HandleMovement
        // return that to the HandleMovement
        Vector3 inputDirection = new (PlayerInputHandler.Instance.MovementInput.x, 0, PlayerInputHandler.Instance.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }
    
    private void HandleJumping()
    {
        // use characterController.isGrounded
        // depreciate the use of raycasts to check ground
        // should be better for performance in long run
        if(characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if(PlayerInputHandler.Instance.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {            
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }
    
    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        // i dont like the fact the HandleJumping method is so nested lmao
        HandleJumping();

        CurrentSpeed = walkSpeed * sprintMultiplier;
        
        characterController.Move(currentMovement * Time.deltaTime);
    }
    
    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }
    
    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
  
    private void HandleRotation()
    {
        float mouseXRotation = PlayerInputHandler.Instance.RotationInput.x * mouseSensitivity;
        float mouseYRotation = PlayerInputHandler.Instance.RotationInput.y * mouseSensitivity;
        
        // stops the player from looking around while trying to use UI
        if (!inMenu)
        {
            ApplyHorizontalRotation(mouseXRotation);
            ApplyVerticalRotation(mouseYRotation);
        }      
    }

    private void HandleCrouch()
    {
        if (PlayerInputHandler.Instance.CrouchTriggered)
        {
            // adjusting the height of collider and character controller
            // the center is changed so that the collider and controller remains on the ground
            col.center = crouchCentre;
            col.height = crouchHeight;
            characterController.center = crouchCentre;
            characterController.height = crouchHeight;

            isCrouching = true;

            cameraAnimator.SetBool("Crouch", true);
        }
        else
        {
            col.center = centre;
            col.height = height;
            characterController.center = centre;
            characterController.height = height;

            isCrouching = false;

            cameraAnimator.SetBool("Crouch", false);
        }
    }

    private void SpeedMultiplierHandler()
    {
        if (PlayerInputHandler.Instance.SprintTriggered && Stamina.Instance.canSprint == true && characterController.isGrounded)
        {
            sprintMultiplier = 2.5f;
        }
        else if (PlayerInputHandler.Instance.CrouchTriggered)
        {
            sprintMultiplier = 0.5f;
        }
        else if (sideKick == true)
        {
            sprintMultiplier = 0.2f;
        }
        else if (!characterController.isGrounded)
        {
            if (PlayerInputHandler.Instance.SprintTriggered && Stamina.Instance.canSprint)
            {
                sprintMultiplier = 1.4f;
            }
            else
            {
                sprintMultiplier = 0.4f;
            }
        }
        else
        {
            // keeps walk speed as original value
            sprintMultiplier = 1f;
        }
    }

    private void WallJump()
    {
        if (characterController.isGrounded)
        {
            canWallJump = false;
            snapKickJump = false;
            wallJumpCounter = 0;
        }
        else if (snapKickJump && canWallJump)
        {
            if (wallJumpCounter < 1)
            {
                currentMovement.y = snapKickJumpForce;
                snapKickJump = false;
                wallJumpCounter += 1;
            }
            else
            {
                Debug.Log("Already wall jumped");
            }    
        }
        else if (sideKickJump && canWallJump)
        {
            if (wallJumpCounter < 1)
            {
                currentMovement.y = sideKickJumpForce;
                sideKickJump = false;
                wallJumpCounter += 1;
            }
            else
            {
                Debug.Log("Already wall jumped");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ResetScene"))
        {
            SceneManager.LoadScene("AlphaPlaytest");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!characterController.isGrounded && hit.transform.CompareTag("Wall"))
        {
            Debug.Log("HitWall");
            canWallJump = true;
        }  
    }
}
