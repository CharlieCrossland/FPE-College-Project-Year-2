using System.Collections;
using UnityEngine;
using UnityEngine.Events;


#pragma warning disable CS0414 // disables this variable not in use crap in unity console
public class Punch : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public UnityEvent CooldownStart;

    [Header("Punch")]
    [SerializeField] private GameObject fist;
    private int punchCounter;
    private float punchCountdown;
    readonly private float maxPunchCountdown = 2f;
    private bool startPunchCounterManager;

    private void Awake()
    {
        fist = GameObject.Find("Fists");
        animator = fist.GetComponent<Animator>();
        punchCountdown = maxPunchCountdown;
    }

    private void Update()
    {
        CanPunch();
        PunchCounterManager();
    }

    void CanPunch()
    {
        if (CombatManager.Instance.attackMethod == "" && CombatManager.Instance.canAttack == true)
        {
            if (PlayerInputHandler.Instance.AttackTriggered)
            {
                startPunchCounterManager = true;
                DetectPunchAttackSequence();
            }
        }

        if (CombatManager.Instance.attackMethod != "")
        {
            fist.SetActive(false);
        }
        else
        {
            fist.SetActive(true);
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
                PunchAttack1();
                break;
            case 1:
                PunchAttack2();
                break;
            case 2:
                PunchAttack3();
                break;
            case 3:
                PunchAttack4();
                break;
        }
         
    }

    private void PunchAttack1()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 1;
        animator.SetTrigger("Punch1");
        CooldownStart.Invoke();
    }

    private void PunchAttack2()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 2;
        animator.SetTrigger("Punch2");
        CooldownStart.Invoke();
    }

    private void PunchAttack3()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch3");
        CooldownStart.Invoke();
    }

    private void PunchAttack4()
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
}