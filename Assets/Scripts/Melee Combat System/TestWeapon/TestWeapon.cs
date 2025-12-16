using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TestWeapon : MonoBehaviour, IInteractable
{
    public string weaponName = "TestWeapon";
    [SerializeField] private Transform PlayerHand;

    [Header("Attacking")]
    [SerializeField] private Animator animator;

    [Header("Cooldown")]
    private float CD = 1f;

    public void Interact()
    {
        CombatManager.Instance.weaponEquipped = true;
    }

    private void Update()
    {
        if (CombatManager.Instance.weaponEquipped == true)
        {
            transform.SetPositionAndRotation(PlayerHand.transform.position, PlayerHand.transform.rotation);
            transform.SetParent(PlayerHand);

            Attacking();
            Drop();
        }
        else
        {
            transform.SetParent(null);
        }
    }

    private void Attacking()
    {
        if (PlayerInputHandler.Instance.attackAction.WasPressedThisFrame() && CombatManager.Instance.canAttack)
        {
            CombatManager.Instance.canAttack = false;
            animator.SetTrigger("Attack");
            StartCoroutine(ResetCD());
        }
    }

    IEnumerator ResetCD()
    {
        yield return new WaitForSeconds(CD);
        CombatManager.Instance.canAttack = true;
        yield break;
    }

    private void Drop()
    {
        if (PlayerInputHandler.Instance.DropTriggered && CombatManager.Instance.canAttack)
        {
            CombatManager.Instance.weaponEquipped = false;
        }
    }
}