using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float noHealth;

    public bool knockback;

    private void Update()
    {
        
    }

    public void BasicPunch()
    {
        health -= CombatManager.Instance.BasicPunchDMG;
    }
}
