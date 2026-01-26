using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DoorMenu : MonoBehaviour, IInteractable
{
    private bool pullDownMap;
    [SerializeField] private GameObject levelSelectScreen;
    private Animator animator;

    public void Interact()
    {
        if (!FirstPersonController.Instance.inMenu)
        {
            levelSelectScreen.SetActive(false);
        }
    }

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (FirstPersonController.Instance.inMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                levelSelectScreen.SetActive(false);
                FirstPersonController.Instance.inMenu = false;
            }
        }
        
        if (pullDownMap)
        {
            StartCoroutine(MapDownTransition());
        }
    }

    IEnumerator MapDownTransition()
    {
        levelSelectScreen.transform.DOMoveY(-6f, 3f);
        yield break;
    }

    public void StartLevel1Coroutine()
    {
        StartCoroutine(Level1());
    }

    IEnumerator Level1()
    {
        pullDownMap = true;
        yield return new WaitForSeconds(2f);
        StartCoroutine(BlackScreen.Instance.StartBlackScreen());
        SceneManager.LoadScene("OfficeLevel1");
        yield break;
    }
}
