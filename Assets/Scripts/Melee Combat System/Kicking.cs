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
        animator = legs.GetComponent<Animator>();
    }

    private void Update()
    {
        CanKick();
    }

    void CanKick()
    {
        if (!CombatManager.Instance.weaponEquipped && CombatManager.Instance.canAttack)
        {
            if (PlayerInputHandler.Instance.kickAction.WasPressedThisFrame())
            {
                CombatManager.Instance.canAttack = false;
                animator.SetTrigger("Attack");
                CooldownStart.Invoke();
            }
        }
    }
}
