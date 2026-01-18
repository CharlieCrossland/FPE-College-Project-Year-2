using Mono.Cecil;
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

    [Header("Raycasts")]
    [SerializeField] private float kickRange;
    [SerializeField] private Transform raySource;
    RaycastHit hit;
    [SerializeField] private LayerMask layerMask;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        legs = GameObject.Find("Legs");
        animator = legs.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        SnapKick();
        RoundhouseKick();
    }

    void SnapKick()
    {
        if (CombatManager.Instance.canAttack && !Punch.Instance.rightJab)
        {
            if (PlayerInputHandler.Instance.kickAction.WasPressedThisFrame())
            {
                CombatManager.Instance.canAttack = false;
                animator.SetTrigger("Attack");
                StartCoroutine(SnapKickRay());
                CooldownStart.Invoke();
            }
        }
    }

    IEnumerator SnapKickRay()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * kickRange, Color.yellow);

        Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
        if (Physics.Raycast(r, out hit, kickRange, layerMask))
        {
            // checking if enemy health script is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
            enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                yield return new WaitForSeconds(0.5f);
                enemyHealth.SnapKick();
                yield break;
            }
            else
            {
                yield break;
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
