using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Kicking : MonoBehaviour
{
    [Header("Kick")]
    [SerializeField] private GameObject legs;
    [SerializeField] private Animator animator;

    [Header("Cooldown")]
    public UnityEvent CooldownStart;

    private void Awake()
    {
        legs = GameObject.Find("Legs");
        animator = legs.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CanKick();
        RoundhouseKick();
    }

    void CanKick()
    {
        if (CombatManager.Instance.canAttack && !Punch.Instance.rightJab)
        {
            if (PlayerInputHandler.Instance.kickAction.WasPressedThisFrame())
            {
                CombatManager.Instance.canAttack = false;
                animator.SetTrigger("Attack");
                CooldownStart.Invoke();
            }
        }
    }

    void RoundhouseKick()
    {
        if (Punch.Instance.rightJab == true)
        {
            StartCoroutine(RightJabCountdown());
        }

        if (CombatManager.Instance.canAttack && Punch.Instance.rightJab)
        {
            if (PlayerInputHandler.Instance.kickAction.WasPressedThisFrame())
            {
                Debug.Log("ROUNDHOUSE KICK");
            }
        }
    }

    IEnumerator RightJabCountdown()
    {
        yield return new WaitForSeconds(1.5f);
        Punch.Instance.rightJab = false;
        yield break;
    }
}
