using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorMenu : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private Animator animator;

    public void Interact()
    {
        StartCoroutine(MapUpTransition());
        levelSelectScreen.SetActive(true);
        FirstPersonController.Instance.inMenu = true;
    }

    private void Start()
    {
        levelSelectScreen.SetActive(false);
    }

    private void Update()
    {
        if (FirstPersonController.Instance.inMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(MapDownTransition());
                FirstPersonController.Instance.inMenu = false;
            }
        }
    }

    IEnumerator MapDownTransition()
    {
        animator.SetTrigger("Down");
        yield return new WaitForSeconds(1.5f);
        levelSelectScreen.SetActive(false);
        yield break;
    }

    IEnumerator MapUpTransition()
    {
        levelSelectScreen.SetActive(true);
        animator.SetTrigger("Up");
        yield return new WaitForSeconds(1.5f);
        yield break;
    }

    public void StartLevel1Coroutine()
    {
        Debug.Log("StartLevel1Coroutine");
        StartCoroutine(Level1());
    }

    IEnumerator Level1()
    {
        StartCoroutine(MapDownTransition());
        StartCoroutine(BlackScreen.Instance.StartBlackScreen());
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("OfficeLevel1");
        yield break;
    }
}