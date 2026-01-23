using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Cooldown")]
    private float cd;
    private float maxCD = 7.5f;

    [Header("Raycast")]
    [SerializeField] private Transform raySource;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask layerMask;
    RaycastHit hit;
    private Health healthScript;



    private void Update()
    {
        Debug.DrawRay(raySource.position, raySource.TransformDirection(Vector3.forward) * attackRange, Color.yellow);

        Cooldown();
        Attack();
    }

    private void Cooldown()
    {
        cd -= Time.deltaTime * 2f;

        if (cd <= 0)
        {
            cd = 0;
            Debug.Log("Enemy Cooldown = 0");
        }
    }

    private void Attack()
    {
        if (transform)
        {
            StartCoroutine(AttackRay());
        }
    }

    IEnumerator AttackRay()
    {
        Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
        if (Physics.Raycast(r, out hit, attackRange, layerMask))
        {
            healthScript = hit.collider.GetComponentInChildren<Health>();
            if (healthScript != null)
            {
                Debug.Log("Enemy Attack");
                healthScript.currentHealth -= 20;
                cd = maxCD;
                yield break;
            }
        }

        if (cd >= maxCD)
        {
            yield break;    
        }

        yield break;
    }
}
