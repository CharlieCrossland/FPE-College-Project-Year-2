using System.Collections;
using UnityEngine;
using UnityEngine.Events;


#pragma warning disable CS0414 // disables this variable not in use crap in unity console
public class Punch : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public UnityEvent CooldownStart;

    [Header("Punch")]
    [SerializeField] private GameObject hands;
    private int punchCounter;
    private float punchCountdown;
    readonly private float maxPunchCountdown = 1f; // cant be less than attack cooldown
    private bool startPunchCounterManager;
    [SerializeField] private float punchRange;

    [Header("Raycasts")]
    [SerializeField] private Transform raySource;
    RaycastHit hit;
    [SerializeField] private LayerMask layerMask;

    private bool sendBasicPunchRay;
    private bool sendHookPunchRay;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        hands = GameObject.Find("Fists");
        animator = hands.GetComponentInChildren<Animator>();
        punchCountdown = maxPunchCountdown;
    }

    private void Update()
    {
        IsWeaponEquipped();
        CanPunch();
        PunchCounterManager();
    }

    private void FixedUpdate()
    {
        BasicPunchRay();
        HookPunchRay();
    }

    void IsWeaponEquipped()
    {
        // if the player does not have a weapon enable fists and allow for punching
        if (!CombatManager.Instance.weaponEquipped)
        {
            hands.SetActive(true);
        }
        else
        {
            hands.SetActive(false);
        }
    }

    void CanPunch()
    {
        if (!CombatManager.Instance.weaponEquipped && CombatManager.Instance.canAttack == true)
        {
            if (PlayerInputHandler.Instance.attackAction.WasPressedThisFrame())
            {
                startPunchCounterManager = true;
                DetectPunchAttackSequence();
            }
        }
    }


    // can possibly change this to be used with all attacks
    // find animator of current attack 
    // if pick up weapon detect weapon and find that animator
    // all animators must use the same trigger names
    private void DetectPunchAttackSequence()
    {
        // use switch case to keep code clean and readable
        // counter starts at 0 as default state
        switch (punchCounter)
        {
            case 0:
                LeftJab();
                break;
            case 1:
                RightJab();
                break;
            case 2:
                RightHook();
                break;
            case 3:
                Uppercut();
                break;
        }
         
    }

    private void LeftJab()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 1;
        animator.SetTrigger("Punch1");
        sendBasicPunchRay = true;
        CooldownStart.Invoke();
    }

    private void RightJab()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 2;
        animator.SetTrigger("Punch2");
        sendBasicPunchRay = true;
        CooldownStart.Invoke();
    }

    private void RightHook()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch3");
        sendHookPunchRay = true;
        CooldownStart.Invoke();
    }

    private void Uppercut()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch4");
        CooldownStart.Invoke();
        StartCoroutine(ResetPunchCombo());
    }

    IEnumerator ResetPunchCombo()
    {
        punchCounter = 0;
        yield break;
    }

    void PunchCounterManager()
    {
        // if this coroutine is played then start countdown
        // when countdown reaches 0 reset counter
        // when countdown is done reset the variables so that it is back to default
        if (startPunchCounterManager == true)
        {
            if (punchCountdown <= 0)
            {
                startPunchCounterManager = false;
            }
            else
            {
                punchCountdown -= Time.deltaTime;
            }
        }
        else
        {
            punchCounter = 0;
            punchCountdown = maxPunchCountdown;
        }    
    }

    private void BasicPunchRay()
    {
        if (sendBasicPunchRay)
        {
            Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
            if (Physics.Raycast(r, out hit, punchRange, layerMask))
            {
                // checking if enemy health is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
                enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.BasicPunch();
                }
                sendBasicPunchRay = false;
            }
            else
            {
                sendBasicPunchRay = false;
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * punchRange, Color.yellow);
        }
    }

    private void HookPunchRay()
    {
        if (sendHookPunchRay)
        {
            Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
            if (Physics.Raycast(r, out hit, punchRange, layerMask))
            {
                // checking if enemy health is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
                enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.HookPunch();
                }
                sendHookPunchRay = false;
            }
            else
            {
                sendHookPunchRay = false;
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * punchRange, Color.yellow);
        }
    }
}