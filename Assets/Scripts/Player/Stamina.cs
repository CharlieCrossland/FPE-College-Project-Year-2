using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public static Stamina Instance;

    [Header("Stamina")]
    public float currentStamina;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaIncreaseMultiplier = 15;
    [SerializeField] private float staminaDecreaseMultiplier = 20;
    public bool canSprint;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaSliderFill;

    [Header("Colours")]
    private Color red = new(1, 0, 0);
    private Color green = new(0, 1, 0);

    private void Awake()
    {
        Instance = this;

        GrabReferences();
        StaminaValueSet();
    }

    void GrabReferences()
    {
        GameObject staminaOBJ = GameObject.Find("StaminaBar");
        staminaSlider = staminaOBJ.GetComponent<Slider>();
        GameObject staminaFillOBJ = GameObject.Find("StaminaFill");
        staminaSliderFill = staminaFillOBJ.GetComponentInChildren<Image>();
    }

    private void StaminaValueSet()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;

        staminaSliderFill.color = green;
    }

    private void Update()
    {
        Main();
    }

    private void Main()
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
            if (!FirstPersonController.Instance.isCrouching) // stops player running when crouched
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

    #region public methods

    public void BasicPunch(float basicPunchStamina = 10f)
    {
        Debug.Log(currentStamina);
        currentStamina -= basicPunchStamina;
    }

    public void HookPunch(float hookPunchStamina = 15f)
    {
        currentStamina -= hookPunchStamina;
    }

    public void UppercutPunch(float uppercutStamina = 20f)
    {
        currentStamina -= uppercutStamina;
    }

    public void SnapKick(float snapKickStamina = 15f)
    {

    }

    public void RoundhouseKick(float roundhouseKickStamina = 30f)
    {

    }

    public void SideKick(float sideKickStamina = 20f)
    {

    }

    #endregion
}
