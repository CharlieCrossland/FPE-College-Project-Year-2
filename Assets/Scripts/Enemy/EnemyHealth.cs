using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    readonly private float noHealth = 0f;

    // knockback will move the enemy opposite to the direction of the punch
    private bool knockback;
    public bool stun; // stun handled in enemy ai script
    public bool dummy;

    private Animator animator;

    [Header("Knockback")]
    [SerializeField] private Transform player;

    [Header("Debug")]
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        health = maxHealth;
    }

    private void Update()
    {
        // Health();
        DebugEnemyUI();
    }

    private void FixedUpdate()
    {
        Health();
    }

    private void Health()
    {
        // what happens when the health reaches 0
        if (health <= noHealth)
        {
            Debug.Log("Enemy Killed");
            //health = noHealth;
            //// play death animation and destroy object
            //Destroy(this.gameObject);

            // ALPHA DELETE AFTER PLAYTEST
            if (dummy)
            {
                StartCoroutine(HealthRegen());
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator HealthRegen()
    {
        Debug.Log("HealthRegen");
        yield return new WaitForSeconds(1f);
        health = maxHealth;
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

    public void CrouchPunch()
    {
        stun = true;

        health -= CombatManager.Instance.BasicPunchDMG;
    }

    public void SnapKick()
    {
        // find the distance between enemy and player
        //Vector3 distance = transform.position.normalized - player.position.normalized;
        // reverse the distance to be opposite of player
        //Vector3 knockbackDirection = -distance;
        //transform.position = knockbackDirection;

        //float opposite = -player.transform.rotation.y;
        //transform.position += new Vector3(opposite, 0, opposite);

        stun = true;

        animator.SetTrigger("UppercutStun");

        health -= CombatManager.Instance.KickDMG;
    }

    private void DebugEnemyUI()
    {
        healthText.SetText("Health: " +  health);
    }
}
