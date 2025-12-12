using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool canAttack = true;
    private float attackCD = 1.0f;

    [Header("Punch")]
    [SerializeField] private GameObject fist;
    private int punchCounter;
    private float punchCountdown;
    readonly private float maxPunchCountdown = 2f;
    private bool startCounterManager;
    private bool noWeapon;

    private void Awake()
    {
        fist = GameObject.Find("Fists");
        animator = fist.GetComponent<Animator>();
        noWeapon = false;
        punchCountdown = maxPunchCountdown;
    }

    private void Update()
    {
        // switch to new input system
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack)
            {
                if (noWeapon == false)
                {
                    startCounterManager = true;
                    DetectPunchAttackSequence();
                }
            }
        }

        PunchCounterManager();
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
        canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 1;
        animator.SetTrigger("Punch1");
        StartCoroutine(ResetAttackCD());
    }

    private void PunchAttack2()
    {
        canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 2;
        animator.SetTrigger("Punch2");
        StartCoroutine(ResetAttackCD());
    }

    private void PunchAttack3()
    {
        canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch3");
        StartCoroutine(ResetAttackCD());
    }

    private void PunchAttack4()
    {
        canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch4");
        StartCoroutine(ResetAttackCD());
        StartCoroutine(ResetPunchCombo());
    }

    IEnumerator ResetAttackCD()
    {
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
        yield break;
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
        if (startCounterManager == true)
        {
            if (punchCountdown <= 0)
            {
                startCounterManager = false;
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
