using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    readonly private float noHealth = 0f;

    // knockback will move the enemy opposite to the direction of the punch
    private bool knockback;
    private bool stun;

    private Animator animator;

    [Header("Debug")]
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Health();
        DebugEnemyUI();
        Stun();
    }

    private void Health()
    {
        // what happens when the health reaches 0
        if (health <= noHealth)
        {
            Debug.Log("Enemy Killed");
            health = noHealth;
            Destroy(this);
        }
    }

    public void BasicPunch()
    {
        health -= CombatManager.Instance.BasicPunchDMG;
    }

    public void HookPunch()
    {
        stun = true;

        health -= CombatManager.Instance.HookDMG;
    }

    public void Uppercut()
    {
        stun = true;

        animator.SetTrigger("UppercutStun");

        health -= CombatManager.Instance.UppercutDMG;
    }

    private void Stun()
    {
        if (stun)
        {
            // pause movement
        }
    }

    private void DebugEnemyUI()
    {
        healthText.SetText("Health: " +  health);
    }
}
