using UnityEngine;
using System.Collections;

public class Kicking : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool canAttack = true;
    private float attackCD = 1.0f;

    [Header("Punch")]
    [SerializeField] private GameObject legs;
    private float kickCountdown;
    readonly private float maxKickCountdown = 2f;

    private void Awake()
    {
        // legs = GameObject.Find("Legs");
        // animator = legs.GetComponent<Animator>();
        kickCountdown = maxKickCountdown;
    }

    private void Update()
    {
        CanKick();
    }

    void CanKick()
    {
        if (CombatManager.Instance.attackMethod == "No Weapon" && canAttack)
        {
            //if (PlayerInputHandler.Instance.KickTriggered)
            //{
                
            //}
        }
    }

    private void PunchAttack1()
    {
        canAttack = false;
        kickCountdown = maxKickCountdown;
    }
}
