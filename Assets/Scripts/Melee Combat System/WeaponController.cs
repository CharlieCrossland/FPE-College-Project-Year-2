using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject fist;
    [SerializeField] private Animator animator;
    private bool canAttack = true;
    private float attackCD = 1.0f;

    private void Awake()
    {
        animator = fist.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        StartCoroutine(ResetAttackCD());
    }

    IEnumerator ResetAttackCD()
    {
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
        yield break;
    }
}
