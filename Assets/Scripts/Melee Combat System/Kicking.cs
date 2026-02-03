using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Kicking : MonoBehaviour
{
    [Header("Kick")]
    [SerializeField] private GameObject legs;
    [SerializeField] private Animator animator;

    private int kickCounter;
    private float kickCountdown;
    readonly private float maxKickCountdown = 2.5f; // cant be less than attack cooldown
    private bool startKickCounterManager;

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
        kickCountdown = maxKickCountdown;
    }

    private void Update()
    {
        CanKick();
        RoundhouseKick();
        KickCounterManager();
    }

    void CanKick()
    {
        if (CombatManager.Instance.canAttack == true)
        {
            if (PlayerInputHandler.Instance.kickAction.WasPressedThisFrame())
            {
                if (!FirstPersonController.Instance.isCrouching)
                {
                    Debug.Log("Kick input detected");
                    startKickCounterManager = true;
                    DetectKickAttackSequence();
                }
                else
                {
                    Sweep();
                }
            }
        }
    }

    void DetectKickAttackSequence()
    {
        // use switch case to keep code clean and readable
        // counter starts at 0 as default state
        switch (kickCounter)
        {
            case 0:
                Debug.Log("KICK");
                SnapKick();
                break;
            case 1:
                SideKick();
                break;
        }
    }

    void SnapKick()
    {
        CombatManager.Instance.canAttack = false;
        kickCountdown = maxKickCountdown;
        kickCounter = +1;
        if (Punch.Instance.rightJab)
        {
            animator.SetTrigger("Roundhouse");
            StartCoroutine(RoundhouseRay());
            Punch.Instance.rightJab = false;
        }
        else
        {
            animator.SetTrigger("SnapKick");
            StartCoroutine(SnapKickRay());
        }
        CooldownStart.Invoke();
    }

    void SideKick()
    {
        CombatManager.Instance.canAttack = false;
        kickCountdown = maxKickCountdown;
        kickCounter = +2;
        FirstPersonController.Instance.sideKick = true;
        animator.SetTrigger("SideKick");
        StartCoroutine(SideKickRay());
        StartCoroutine(ResetSideKickSpeed());
        CooldownStart.Invoke();
        StartCoroutine(ResetKickCombo());
    }

    void FlyingKick()
    {
        CombatManager.Instance.canAttack = false;
        kickCountdown = maxKickCountdown;
        kickCounter = +2;
        animator.SetTrigger("FlyingKick");
        // StartCoroutine(SideKickRay());
        CooldownStart.Invoke();
        StartCoroutine(ResetKickCombo());
    }

    void Sweep()
    {

    }

    IEnumerator ResetKickCombo()
    {
        kickCounter = 0;
        yield break;
    }

    void KickCounterManager()
    {
        // if this coroutine is played then start countdown
        // when countdown reaches 0 reset counter
        // when countdown is done reset the variables so that it is back to default
        if (startKickCounterManager == true)
        {
            if (kickCountdown <= 0)
            {
                startKickCounterManager = false;
            }
            else
            {
                kickCountdown -= Time.deltaTime;
            }
        }
        else
        {
            kickCounter = 0;
            kickCountdown = maxKickCountdown;
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
                yield return new WaitForSeconds(0.4f);
                FirstPersonController.Instance.snapKick = true;
                yield break;
            }
        }
    }

    IEnumerator SideKickRay()
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
                enemyHealth.SideKick();
                yield break;
            }
            else
            {
                yield break;
            }
        }
    }

    IEnumerator ResetSideKickSpeed()
    {
        yield return new WaitForSeconds(0.75f);
        FirstPersonController.Instance.sideKick = false;
        yield break;
    }

    IEnumerator RoundhouseRay()
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
                enemyHealth.SideKick();
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.4f);
                FirstPersonController.Instance.sideKick = true;
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
    }

    IEnumerator RightJabCountdown()
    {
        yield return new WaitForSeconds(1.5f);
        Punch.Instance.rightJab = false;
        yield break;
    }
}
