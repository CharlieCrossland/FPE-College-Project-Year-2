using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // this allows for the script to be referenced anywhere, use PlayerInputHandler.Instance
    public static PlayerInputHandler Instance;

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;
    
    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";
    
    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string kick = "Kick";
    [SerializeField] private string drop = "Drop";

    [Header("Input Actions")]
    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    public InputAction attackAction;
    public InputAction kickAction;
    private InputAction dropAction;
    
    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered;
    public bool CrouchTriggered;
    public bool InteractTriggered { get; private set; }
    public bool AttackTriggered { get; private set; }
    public bool KickTriggered { get; private set; }
    public bool DropTriggered { get; private set; }


    private void Awake()
    {
        Instance = this;

        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);

        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);
        crouchAction = mapReference.FindAction(crouch);
        interactAction = mapReference.FindAction(interact);
        attackAction = mapReference.FindAction(attack);
        kickAction = mapReference.FindAction(kick);
        dropAction = mapReference.FindAction(drop);

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;
    
        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;

        crouchAction.performed += inputInfo => CrouchTriggered = true;
        crouchAction.canceled += inputInfo => CrouchTriggered = false;

        interactAction.performed += inputInfo => InteractTriggered = true;
        interactAction.canceled += inputInfo => InteractTriggered = false;

        attackAction.performed += inputInfo => AttackTriggered = true;
        attackAction.canceled += inputInfo => AttackTriggered = false;

        kickAction.performed += inputInfo => KickTriggered = true;
        kickAction.canceled += inputInfo => KickTriggered = false;

        dropAction.performed += inputInfo => DropTriggered = true;
        dropAction.canceled += inputInfo => DropTriggered = false;
    }
    
    private void OnEnable() 
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }
    
    private void OnDisable() 
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }    

}
