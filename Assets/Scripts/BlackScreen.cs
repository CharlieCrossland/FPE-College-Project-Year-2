using UnityEngine;
using System.Collections;

public class BlackScreen : MonoBehaviour
{
    public static BlackScreen Instance;
    public Animator animator;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        StartCoroutine(EndBlackScreen());
    }

    public IEnumerator StartBlackScreen()
    {
        animator.SetTrigger("Start");
        yield break;
    }

    public IEnumerator EndBlackScreen()
    {
        animator.SetTrigger("End");
        yield break;
    }
}
