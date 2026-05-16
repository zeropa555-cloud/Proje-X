using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthBar; // Inspector'dan UI Slider'ı sürükle
    public Gradient healthGradient; // Renk değişimi için (yeşil→sarı→kırmızı)
    public Image fillImage; // Slider'ın içindeki Fill Area > Fill objesi

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 0'ın altına düşmesin

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

            // Renk değişimi
            if (fillImage != null && healthGradient != null)
            {
                fillImage.color = healthGradient.Evaluate(healthBar.normalizedValue);
            }
        }
    }

    void Die()
    {
        Debug.Log("Öldün kanka!");
        // Buraya ölüm animasyonu, ekran kararma, restart menüsü falan eklersin
        // Şimdilik karakteri devre dışı bırakalım:
        gameObject.SetActive(false);
    }
}