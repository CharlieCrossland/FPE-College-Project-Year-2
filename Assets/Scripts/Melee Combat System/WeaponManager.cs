using UnityEngine;
using System.Collections;

#pragma warning disable CS0414 // disables this variable not in use crap in unity console
public class WeaponManager : MonoBehaviour
{
    private GameObject weaponOBJ;
    private Animator animator;
    private Vector3 playerHandPos;
    private bool canAttack = true;
    private float attackCD = 1.0f;
    [SerializeField] private Transform playerHand;

    private void Update()
    {
        CanAttack();

        if (CombatManager.Instance.setWeapon == true)
        {
            SetWeapon();
        }
    }

    private void SetWeapon()
    {
        weaponOBJ = CombatManager.Instance.currentWeaponOBJ;
        animator = weaponOBJ.GetComponent<Animator>();
        CombatManager.Instance.setWeapon = false;
    }

    void CanAttack()
    {
        if (CombatManager.Instance.attackMethod == "Weapon" && canAttack == true)
        {
            if (PlayerInputHandler.Instance.AttackTriggered)
            {
                canAttack = false;
                animator.SetTrigger("Attack");
                StartCoroutine(ResetAttackCD());
            }
        }
    }

    IEnumerator ResetAttackCD()
    {
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
        yield break;
    }
}
