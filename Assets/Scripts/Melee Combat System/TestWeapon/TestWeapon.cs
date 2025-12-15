using UnityEngine;

public class TestWeapon : MonoBehaviour, IInteractable
{
    public string weaponName = "TestWeapon";
    [SerializeField] private Transform PlayerHand;

    public void Interact()
    {
        CombatManager.Instance.currentWeapon = weaponName;
        CombatManager.Instance.WeaponPickUp();
        transform.position = PlayerHand.transform.position;
        transform.rotation = PlayerHand.transform.rotation;
        transform.SetParent(PlayerHand);
    }
}