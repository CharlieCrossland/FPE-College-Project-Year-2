using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Player")]
    public float currentHealth;
    public float maxHealth;

    [Header("Health Pack")]
    [SerializeField] private float healthPackIncreaseAmount;

    private BoxCollider col;

    [SerializeField] private Slider slider;

    private void Awake()
    {
        currentHealth = maxHealth;
        col = GetComponent<BoxCollider>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health"))
        {
            currentHealth += healthPackIncreaseAmount;
            Stamina.Instance.startCoffeeTimer = true;
            other.gameObject.SetActive(false);
        }
    }
}
