using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public string tutorialInfo;
    public TMP_Text text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            text.SetText(tutorialInfo);
            this.gameObject.SetActive(false);
        }
    }
}
