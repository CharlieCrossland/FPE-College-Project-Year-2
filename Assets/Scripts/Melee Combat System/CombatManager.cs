using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public string attackMethod;
    public string currentWeapon;
    public GameObject currentWeaponOBJ;
    private bool weaponDetected;
    public bool setWeapon;

    [Header("Cooldown")]
    public bool canAttack;
    private float attackCD = 1.0f;

    // weapons available
    public GameObject TestWeaponOBJ;

    private void Awake()
    {
        Instance = this;
        canAttack = true;

        DetectWeapon();
    }

    public void WeaponPickUp()
    {
        if (weaponDetected == false)
        {
            DetectWeapon();
            if (currentWeapon == null)
            {
                currentWeapon = new string("No Weapon");
            }
        }
    }

    private void DetectWeapon()
    {
        switch (currentWeapon)
        {
            case "TestWeapon":
                attackMethod = "Weapon";
                setWeapon = true;
                currentWeaponOBJ = TestWeaponOBJ;
                weaponDetected = true;
                break;
        }
    }

    public void StartCooldown()
    {
        StartCoroutine(ResetAttackCD());
    }

    IEnumerator ResetAttackCD()
    {
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
        yield break;
    }
}
