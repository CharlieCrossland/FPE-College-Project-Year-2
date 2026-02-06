using TMPro;
using UnityEngine;

public class JumpInfo : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text text1;

    // Update is called once per frame
    void Update()
    {
        text.SetText("sprintMultiplier: " + FirstPersonController.Instance.sprintMultiplier);
        text1.SetText("Current Speed: " + FirstPersonController.Instance.CurrentSpeed);
    }
}
