using UnityEngine;
using TMPro;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform InteractorSource;
    [SerializeField] private float InteractRange;

    [Header("WeaponTooltips")]
    [SerializeField] private TMP_Text tooltip;

    [Header("Layers")]
    private int LayerWeapon;
    private int LayerInteract;

    private void Awake()
    {
        // weapon layer
        LayerWeapon = LayerMask.NameToLayer("Weapon");
        LayerInteract = LayerMask.NameToLayer("Interact");

        tooltip.enabled = false;
    }

    void Update()
    {
        InteractRay();
        TooltipDetectionRay();
    }

    void InteractRay()
    {
        if (PlayerInputHandler.Instance.InteractTriggered)
        {
            Ray r = new (InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }

    void TooltipDetectionRay()
    {
        // this runs seperate from the interact ray due to interact ray requiring players input
        // detects weapon object and grabs the script
        Ray detectLayerRay = new (InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(detectLayerRay, out RaycastHit layerInfo, InteractRange))
        {
            // this is for detection on the weapon layer, can use other layers like an interact layer so that I can give a tooltip when the player can interact with a button
            if (layerInfo.collider.gameObject.layer == LayerWeapon && !CombatManager.Instance.weaponEquipped)
            {
                // every weapon requires the same script as that is what we are trying to reach
                if (layerInfo.collider.gameObject.TryGetComponent(out WeaponScript weaponScript))
                {
                    // enables the weapon pickup tooltip and tells the player what weapon it is
                    tooltip.enabled = true;
                    tooltip.SetText("Press E to Pick Up " + weaponScript.weaponName);
                }
            }
            else if (layerInfo.collider.gameObject.layer == LayerInteract && !FirstPersonController.Instance.inMenu)
            {
                tooltip.enabled = true;
                tooltip.SetText("Press E to Interact");
            }
            else
            {
                tooltip.enabled = false;
            }
        }
    }
}