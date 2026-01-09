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
    [SerializeField] private RaycastHit hit;

    private Rigidbody rb;

    public void Interact()
    {
        Debug.Log("Interact:TestWeapon");
        if (!CombatManager.Instance.weaponEquipped)
        {
            inHand = true;
            CombatManager.Instance.weaponEquipped = true;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (inHand)
        {
            //transform.SetPositionAndRotation(PlayerHand.transform.position, PlayerHand.transform.rotation);

            transform.localPosition = PlayerHand.transform.position;
            transform.rotation = PlayerHand.transform.rotation;
            transform.SetParent(PlayerHand);

            animator.SetBool("Idle", false);
            rb.useGravity = false;

            Attacking();
            Drop();
        }
        else
        {
            NotInHand();
        }
    }

    private void NotInHand()
    {
        animator.SetBool("Idle", true);

        if (weaponDropped == true)
        {
            StartCoroutine(SetFloorPosition());
        }
    }

    IEnumerator SetFloorPosition()
    {
        transform.SetParent(null);
        transform.rotation = new Quaternion(0.70711f, 0f, 0f, -0.7071036f);
        transform.position = PlayerHand.transform.position;
        rb.useGravity = true;

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f, layerMask);

        if (hit.collider)
        {
            rb.useGravity = false;
        }

        weaponDropped = false;
        yield break;
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