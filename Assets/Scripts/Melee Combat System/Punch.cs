using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Punch : MonoBehaviour
{
    public static Punch Instance;

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

    [Header("RoundhouseKick")]
    [HideInInspector] public bool rightJab;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        Instance = this;
        hands = GameObject.Find("Fists");
        animator = hands.GetComponentInChildren<Animator>();
        punchCountdown = maxPunchCountdown;
    }

    private void Update()
    {
        IsWeaponEquipped();
        CanPunch();
        PunchCounterManager();
        SecretEmote();
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
                if (!FirstPersonController.Instance.isCrouching)
                {
                    startPunchCounterManager = true;
                    DetectPunchAttackSequence();
                }
                else
                {
                    CrouchPunch();
                }
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
        StartCoroutine(BasicPunchRay());
        CooldownStart.Invoke();
    }

    private void RightJab()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 2;
        animator.SetTrigger("Punch2");
        rightJab = true;
        StartCoroutine(BasicPunchRay());
        CooldownStart.Invoke();
    }

    private void RightHook()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch3");
        StartCoroutine(HookPunchRay());
        CooldownStart.Invoke();
    }

    private void Uppercut()
    {
        CombatManager.Instance.canAttack = false;
        punchCountdown = maxPunchCountdown;
        punchCounter =+ 3;
        animator.SetTrigger("Punch4");
        StartCoroutine(UppercutRay());
        CooldownStart.Invoke();
        StartCoroutine(ResetPunchCombo());
    }

    IEnumerator ResetPunchCombo()
    {
        punchCounter = 0;
        yield break;
    }

    private void CrouchPunch()
    {
        CombatManager.Instance.canAttack = false;
        animator.SetTrigger("Punch2");
        StartCoroutine(CrouchPunchRay());
        CooldownStart.Invoke();
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
            rightJab = false;
            punchCountdown = maxPunchCountdown;
        }    
    }

    #region Coroutines

    IEnumerator BasicPunchRay()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * punchRange, Color.yellow);
        Stamina.Instance.BasicPunch();

        Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
        if (Physics.Raycast(r, out hit, punchRange, layerMask))
        {
            // checking if enemy health script is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
            enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                yield return new WaitForSeconds(0.5f);
                enemyHealth.BasicPunch();
                yield break;
            }
            else if (hit.collider.gameObject.CompareTag("Breakable"))
            {
                yield return new WaitForSeconds(0.5f);
                hit.collider.gameObject.SetActive(false);
            }
            else
            {
                yield break;
            }
        }
    }

    private IEnumerator HookPunchRay()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * punchRange, Color.yellow);
        Stamina.Instance.HookPunch();

        Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
        if (Physics.Raycast(r, out hit, punchRange, layerMask))
        {
            // checking if enemy health script is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
            enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                yield return new WaitForSeconds(0.5f);
                enemyHealth.HookPunch();
                yield break;
            }
            else if (hit.collider.gameObject.CompareTag("Breakable"))
            {
                yield return new WaitForSeconds(0.5f);
                hit.collider.gameObject.SetActive(false);
            }
            else
            {
                yield break;
            }
        }
    }

    private IEnumerator UppercutRay()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * punchRange, Color.yellow);
        Stamina.Instance.UppercutPunch();

        Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
        if (Physics.Raycast(r, out hit, punchRange, layerMask))
        {
            // checking if enemy health script is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
            enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                yield return new WaitForSeconds(0.75f);
                enemyHealth.Uppercut();
                yield break;
            }
            else if (hit.collider.gameObject.CompareTag("Breakable"))
            {
                yield return new WaitForSeconds(0.75f);
                hit.collider.gameObject.SetActive(false);
            }
            else
            {
                yield break;
            }
        }
    }

    private IEnumerator CrouchPunchRay()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * punchRange, Color.yellow);
        Stamina.Instance.BasicPunch();

        Ray r = new(raySource.position, raySource.TransformDirection(Vector3.forward));
        if (Physics.Raycast(r, out hit, punchRange, layerMask))
        {
            // checking if enemy health script is not null otherwise an error is thrown that the component is missing while it is trying to be accessed
            enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                yield return new WaitForSeconds(0.5f);
                enemyHealth.CrouchPunch();
                yield break;
            }
            else
            {
                yield break;
            }
        }
    }

    #endregion

    #region Secret Emote

    bool canSecretEmote;
    private void SecretEmote()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(SecretEmoteCountdown());
        }

        if (canSecretEmote && Input.GetKeyDown(KeyCode.Alpha7))
        {
            Stamina.Instance.currentStamina = 0;
            animator.SetTrigger("SecretEmote");
            canSecretEmote = false;
        }
    }

    IEnumerator SecretEmoteCountdown()
    {
        canSecretEmote = true;
        yield return new WaitForSeconds(3f);
        canSecretEmote = false;
        yield break;
    }

    #endregion
}