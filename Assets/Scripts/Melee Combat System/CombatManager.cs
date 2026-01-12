using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("Detect Weapon")]
    public bool weaponEquipped;

    [Header("Cooldown")]
    public bool canAttack;
    private float punchCD = 0.55f;
    private float kickCD = 1.0f;


    private void Awake()
    {
        Instance = this;
        canAttack = true;
    }

    public void PunchCooldown()
    {
        StartCoroutine(ResetPunchCD());
    }

    IEnumerator ResetPunchCD()
    {
        yield return new WaitForSeconds(punchCD);
        canAttack = true;
        yield break;
    }

    public void KickCooldown()
    {
        StartCoroutine(ResetKickCD());
    }

    IEnumerator ResetKickCD()
    {
        yield return new WaitForSeconds(kickCD);
        canAttack = true;
        yield break;
    }
}
