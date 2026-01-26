using UnityEngine;

public class DoorMenu : MonoBehaviour, IInteractable
{
    private bool inMenu;
    private Canvas levelSelect;

    public void Interact()
    {
        if (!inMenu)
        {
            levelSelect.enabled = true;
        }
    }

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (inMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                levelSelect.enabled = false;
                inMenu = false;
            }
        }
    }
}
