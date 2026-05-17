using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthBar;
    public Gradient healthGradient;
    public Image fillImage;

    void Awake()
    {
        // GameManager tarafından yönetilecek
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // GameManager'ın canı okuması için
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // GameManager'ın canı geri yüklemesi için
    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            if (fillImage != null && healthGradient != null)
            {
                fillImage.color = healthGradient.Evaluate(healthBar.normalizedValue);
            }
        }
    }

    void Die()
    {
        Debug.Log("Öldün kanka!");
        gameObject.SetActive(false);
    }
}