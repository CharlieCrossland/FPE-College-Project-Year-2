using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveTutorial : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
