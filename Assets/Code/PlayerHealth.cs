using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthBar;
    public Gradient healthGradient;
    public Image fillImage;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public int GetCurrentHealth() => currentHealth;

    public void SetHealth(int hp)
    {
        currentHealth = Mathf.Clamp(hp, 0, maxHealth);
        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("☠️ Öldün! Chapter 1 başa sarılıyor...");

        // ⬇️ BURASI EKLENDİ - GameManager'a haber ver
        if (GameManager.Instance != null)
            GameManager.Instance.ResetChapter();
        else
            gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            if (fillImage != null && healthGradient != null)
                fillImage.color = healthGradient.Evaluate(healthBar.normalizedValue);
        }
    }
}