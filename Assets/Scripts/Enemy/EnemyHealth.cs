using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    private float maxHealth;
    private float noHealth = 0f;

    // knockback will move the enemy opposite to the direction of the punch
    private bool knockback;
    private bool hookStun;

    [Header("Debug")]
    [SerializeField] private TMP_Text healthText;

    private void Start()
    {
        maxHealth = health;
    }

    private void Update()
    {
        Health();
        DebugEnemyUI();
    }

    private void Health()
    {
        // what happens when the health reaches 0
        if (health <= noHealth)
        {
            Debug.Log("Enemy Killed");
            health = noHealth;
        }
    }

    public void BasicPunch()
    {
        health -= CombatManager.Instance.BasicPunchDMG;
    }

    public void HookPunch()
    {
        hookStun = true;

        if (hookStun)
        {
            // stop movement shortly
        }
    }

    private void DebugEnemyUI()
    {
        healthText.SetText("Health: " +  health);
    }
}
