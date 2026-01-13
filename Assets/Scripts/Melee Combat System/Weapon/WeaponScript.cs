using System.Collections;
using UnityEngine;

#pragma warning disable IDE0044 // disables "make this field readonly" message
public class WeaponScript : MonoBehaviour, IInteractable
{
    public string weaponName = "Test Weapon";
    [SerializeField] private Transform PlayerHand;

    [Header("Animations")]
    [SerializeField] private Animator animator;

    [Header("Cooldown")]
    private float CD = 1f;

    [Header("Floor Detection")]
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;
    private bool onFloor;
    [SerializeField] private float gravity = 2f;
    private bool weaponDropped;
    private bool inHand;
    private float distance = 0.35f;

    [Header("Weapon Variables")]
    public float Damage;
    public float AttackSpeed;
    public float Durability;


    public void Interact()
    {
        if (!CombatManager.Instance.weaponEquipped)
        {
            inHand = true;
            CombatManager.Instance.weaponEquipped = true;
        }
    }

    private void Update()
    {
        if (inHand)
        {
            InHand();
        }
        else
        {
            NotInHand();
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * distance, Color.green);
    }

    private void InHand()
    {
        transform.SetPositionAndRotation(PlayerHand.transform.position, PlayerHand.transform.rotation);
        transform.SetParent(PlayerHand);

        animator.SetBool("Idle", false);
        onFloor = false;
        weaponDropped = false;

        Attacking();
        Drop();
    }

    private void NotInHand()
    {
        if (weaponDropped == true)
        {
            StartCoroutine(SetFloorPosition());
        }
        // If the weapon has not been dropped and is not in the players hand, it must be on the floor.
        else if (weaponDropped == false && inHand == false)
        {
            onFloor = true;
        }

        // when hitting the floor, it will now remain static and play idle animation on model
        if (onFloor)
        {
            animator.SetBool("Idle", true);
        }
    }

    IEnumerator SetFloorPosition()
    {
        transform.SetParent(null);
        transform.rotation = new Quaternion(0f, 0f, 0f, 1);
        // transform.position = PlayerHand.transform.position;
        

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, distance, layerMask);

        // use minus one to move object down and times by gravity
        transform.position += new Vector3(0, -gravity * Time.deltaTime, 0);

        if (hit.collider)
        {
            weaponDropped = false;
            onFloor = true;
            gravity = 2f;
            yield break;
        }
        else
        {
            gravity += 0.5f;
        }
    }

    private void Attacking()
    {
        if (PlayerInputHandler.Instance.attackAction.WasPressedThisFrame() && CombatManager.Instance.canAttack)
        {
            CombatManager.Instance.canAttack = false;
            animator.SetTrigger("Attack");
            StartCoroutine(ResetCD());
        }
    }

    IEnumerator ResetCD()
    {
        yield return new WaitForSeconds(CD);
        CombatManager.Instance.canAttack = true;
        yield break;
    }

    private void Drop()
    {
        if (PlayerInputHandler.Instance.DropTriggered && CombatManager.Instance.canAttack)
        {
            CombatManager.Instance.weaponEquipped = false;
            inHand = false;
            weaponDropped = true;
        }
    }
}