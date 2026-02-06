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
    }

    private void Start()
    {
        levelSelectScreen.SetActive(false);
    }

    IEnumerator MapDownTransition()
    {
        Debug.Log("Map Down");
        animator.SetTrigger("Down");
        FirstPersonController.Instance.inMenu = false;
        yield return new WaitForSeconds(1.5f);
        levelSelectScreen.SetActive(false);
        yield break;
    }

    IEnumerator MapUpTransition()
    {
        Debug.Log("Map Up");
        levelSelectScreen.SetActive(true);
        animator.SetTrigger("Up");
        FirstPersonController.Instance.inMenu = true;
        yield return new WaitForSeconds(1.5f);
        yield break;
    }

    public void StartTutorialCoroutine()
    {
        StartCoroutine(Tutorial());
    }

    IEnumerator Tutorial()
    {
        StartCoroutine(MapDownTransition());
        StartCoroutine(BlackScreen.Instance.StartBlackScreen());
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Tutorial");
        yield break;
    }

    public void StartLevel1Coroutine()
    {
        StartCoroutine(Level1());
    }

    IEnumerator Level1()
    {
        StartCoroutine(MapDownTransition());
        //StartCoroutine(BlackScreen.Instance.StartBlackScreen());
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("OfficeLevel1");
        yield break;
    }

    public void Exit()
    {
        StartCoroutine(MapDownTransition());
    }
}