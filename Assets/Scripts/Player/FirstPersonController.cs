using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

// add components needed for script to work
// prevents compile errors
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Stamina")]
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaIncreaseMultiplier = 0.2f;
    [SerializeField] private float staminaDecreaseMultiplier = 0.4f;
    [SerializeField] private bool startStaminaIncrease;
    [SerializeField] private bool canSprint;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaSliderFill;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CinemachineCamera mainCamera;

    [Header("Colours")]
    private Color red = new Color(1, 0, 0);
    private Color green = new Color(0, 1, 0);

    private Vector3 currentMovement;
    private float verticalRotation;

    // directly manipulate the variable
    private float CurrentSpeed => walkSpeed * (PlayerInputHandler.Instance.SprintTriggered ? sprintMultiplier : 1);

    private void Awake()
    {
        CheckReferences();
        StaminaValueSet();
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
        }
    }

    bool CheckNull()
    {
        // variables may be set as null/are missing
        // these are failsafes incase they do
        if (characterController || mainCamera == null)
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
        Stamina();
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

    private void Stamina()
    {
        if (PlayerInputHandler.Instance.SprintTriggered && canSprint == true)
        {
            sprintMultiplier = 2f;

            currentStamina -= staminaDecreaseMultiplier;
        }
        else
        {
            // use else for anything that isnt sprint and bool canSprint true
            // keeps walk speed as original value
            sprintMultiplier = 1f;
            startStaminaIncrease = true;

            StaminaIncrease();
        }

        staminaSlider.value = currentStamina;

        StaminaCap();
        StaminaSliderColourChange();
    }

    private void StaminaIncrease()
    {
        if (startStaminaIncrease)
        {
            currentStamina += staminaIncreaseMultiplier;
        }
    }

    private void StaminaCap()
    {
        // stops stamina going above max stamina
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
            startStaminaIncrease = false;
        }

        // stops current stamina going below 0
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            canSprint = false;
            PlayerInputHandler.Instance.SprintTriggered = false;
            startStaminaIncrease = true;
        }

        if (currentStamina > 25)
        {
            canSprint = true;
        }
    }

    private void StaminaSliderColourChange()
    {
        if (currentStamina < 25)
        {
            staminaSliderFill.color = red;
        }
        else
        {
            staminaSliderFill.color = green;
        }
    }
}
