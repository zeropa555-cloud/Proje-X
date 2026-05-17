using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int dmg)
    {
        Debug.Log("💔 TakeDamage çağrıldı! Hasar: " + dmg + " | Obje: " + gameObject.name);

        if (currentHealth <= 0) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (anim != null)
            anim.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("💀 Düşman öldü: " + gameObject.name);

        // AI'yi kapat
        EnemyAI ai1 = GetComponent<EnemyAI>();
        EnemyAI2 ai2 = GetComponent<EnemyAI2>();
        if (ai1 != null) ai1.Die();
        if (ai2 != null) ai2.Die();

        if (anim != null)
            anim.SetTrigger("Die");

        // Sahne geçişi
        if (GameManager.Instance != null)
            GameManager.Instance.GoToNextLevel();

        Destroy(gameObject, 3f);
    }

    public int GetCurrentHealth() => currentHealth;
}