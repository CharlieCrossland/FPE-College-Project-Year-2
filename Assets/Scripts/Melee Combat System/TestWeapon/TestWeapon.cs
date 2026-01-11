using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

#pragma warning disable IDE0044 // disables "make this field readonly" message
public class TestWeapon : MonoBehaviour, IInteractable
{
    public string weaponName = "TestWeapon";
    [SerializeField] private Transform PlayerHand;
    private bool inHand;
    private bool weaponDropped;

    [Header("Animations")]
    [SerializeField] private Animator animator;

    [Header("Cooldown")]
    private float CD = 1f;

    [Header("Floor Detection")]
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;
    private bool onFloor;
    [SerializeField] private float gravity = 2f;

    public void Interact()
    {
        Debug.Log("Interact:TestWeapon");
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
            //transform.SetPositionAndRotation(PlayerHand.transform.position, PlayerHand.transform.rotation);

            transform.position = PlayerHand.transform.position;
            transform.rotation = PlayerHand.transform.rotation;
            transform.SetParent(PlayerHand);

            animator.SetBool("Idle", false);
            onFloor = false;
            weaponDropped = false;

            Attacking();
            Drop();
        }
        else
        {
            NotInHand();
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.2f, Color.green);
    }

    private void NotInHand()
    {
        if (weaponDropped == true)
        {
            StartCoroutine(SetFloorPosition());
        }
        else if (weaponDropped == false && inHand == false)
        {
            onFloor = true;
        }

        if (onFloor)
        {
            animator.SetBool("Idle", true);
        }
    }

    IEnumerator SetFloorPosition()
    {
        transform.SetParent(null);
        transform.rotation = new Quaternion(0.70711f, 0f, 0f, -0.7071036f);
        transform.position = PlayerHand.transform.position;

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.2f, layerMask);

        // use minus one to move object down and times by gravity
        transform.position += new Vector3(transform.position.x,-1 * gravity, transform.position.z);

        if (hit.collider)
        {
            weaponDropped = false;
            onFloor = true;
            yield break;
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