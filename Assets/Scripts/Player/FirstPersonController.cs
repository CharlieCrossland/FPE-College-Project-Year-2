using UnityEngine;
using Unity.Cinemachine;

// add components needed for script to work
// prevents compile errors
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CinemachineCamera mainCamera;

    private Vector3 currentMovement;
    private float verticalRotation;

    // directly manipulate the variable
    private float CurrentSpeed => walkSpeed * (PlayerInputHandler.Instance.SprintTriggered ? sprintMultiplier : 1);

    private void Awake()
    {
        CheckReferences();
    }

    void CheckReferences()
    {
        // avoid anything missing that could break the game
        // a really awful way to do this but i wanted to learn how to use returns
        // bite me
        bool allReferencesGrabbed = CheckNull();

        if (allReferencesGrabbed == true)
        {
            return;
        }
        if (allReferencesGrabbed == false)
        {
            characterController = GetComponent<CharacterController>();
            mainCamera = GetComponentInChildren<CinemachineCamera>();
            return;
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

        // i dont like the fact HandleJumping is so hidden and nested lmao
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
    
}
