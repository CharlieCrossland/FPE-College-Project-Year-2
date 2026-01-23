using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    [SerializeField] private Slider slider;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        HealthCap();
        Death();
        SliderUI();
    }

    private void HealthCap()
    {
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }

    private void Death()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("PlayerDead");
        }
    }

    private void SliderUI()
    {
        slider.value = currentHealth;
        slider.maxValue = maxHealth;
    }
}
