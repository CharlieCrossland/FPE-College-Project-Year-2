using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// add components needed for script to work
// prevents compile errors
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController Instance;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier;

    [Header("Stamina")]
    public float currentStamina;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaIncreaseMultiplier = 0.2f;
    [SerializeField] private float staminaDecreaseMultiplier = 0.4f;
    [SerializeField] private bool canSprint;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaSliderFill;

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
    private bool canWallJump;
    [SerializeField] private float wallJumpForce;
    public bool snapKick;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CapsuleCollider col;

    [Header("Animators")]
    [SerializeField] private Animator cameraAnimator;

    [Header("Colours")]
    private Color red = new (1, 0, 0);
    private Color green = new (0, 1, 0);

    private Vector3 currentMovement;
    private float verticalRotation;

    // directly manipulate the variable
    private float CurrentSpeed;

    private void Awake()
    {
        CheckReferences();
        StaminaValueSet();

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

    private void StaminaValueSet()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;

        staminaSliderFill.color = green;
    }

    void Start()
    {
        SetCursor();
    }

    void SetCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleCrouch();
        WallJump();
        Stamina();
        SpeedMultiplierHandler();
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
        Stamina();

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
        
        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);        
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
        if (PlayerInputHandler.Instance.SprintTriggered && canSprint == true)
        {
            sprintMultiplier = 2f;
        }
        else if (PlayerInputHandler.Instance.CrouchTriggered)
        {
            sprintMultiplier = 0.5f;
        }
        else
        {
            // keeps walk speed as original value
            sprintMultiplier = 1f;
        }
    }

    private void Stamina()
    {
        if (PlayerInputHandler.Instance.SprintTriggered && canSprint == true)
        {
            StaminaDecrease();
        }
        else
        {
            StaminaIncrease();
        }

        // sprinting then crouching would register the player as still running
        if (PlayerInputHandler.Instance.SprintTriggered)
        {
            if (PlayerInputHandler.Instance.CrouchTriggered)
            {
                PlayerInputHandler.Instance.SprintTriggered = false;
            }
        }

        StaminaCap();
        StaminaSlider();
    }
    private void StaminaIncrease()
    {
        currentStamina += (staminaIncreaseMultiplier * Time.deltaTime);
    }

    private void StaminaDecrease()
    {
        currentStamina -= (staminaDecreaseMultiplier * Time.deltaTime);
    }

    private void StaminaCap()
    {
        // stops stamina going above max stamina
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }

        // stops current stamina going below 0
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            canSprint = false;
            PlayerInputHandler.Instance.SprintTriggered = false;
        }

        if (currentStamina > 25)
        {
            if (!isCrouching) // stops player running when crouched
            {
                canSprint = true;
            }
        }
    }

    private void StaminaSlider()
    {
        staminaSlider.value = currentStamina;

        if (currentStamina < 25)
        {
            staminaSliderFill.color = red;
        }
        else
        {
            staminaSliderFill.color = green;
        }
    }

    private void WallJump()
    {
        if (characterController.isGrounded)
        {
            canWallJump = false;
        }
        else if (snapKick && canWallJump)
        {
            currentMovement.y = wallJumpForce;
            snapKick = false;
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
