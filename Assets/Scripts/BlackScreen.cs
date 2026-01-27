using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    public static BlackScreen Instance;
    public Animator animator;
    [SerializeField] private Image blackScreen;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        blackScreen.enabled = true;
        StartCoroutine(EnterLevel());
    }

    public IEnumerator StartBlackScreen()
    {
        animator.SetTrigger("Start");
        yield break;
    }

    public IEnumerator EnterLevel()
    {
        animator.SetTrigger("End");
        yield break;
    }
}
