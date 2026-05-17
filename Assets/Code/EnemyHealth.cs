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
        if (currentHealth <= 0) return;
        currentHealth -= dmg;
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

        // ⬇️ BURASI EKLENDİ - ArenaManager'a haber ver!
        ArenaManager arena = FindObjectOfType<ArenaManager>();
        if (arena != null)
        {
            arena.OnEnemyDied(gameObject);
            Debug.Log("📢 ArenaManager'a haber verildi!");
        }
        else
        {
            Debug.LogWarning("⚠️ ArenaManager bulunamadı!");
        }

        Destroy(gameObject, 3f);
    }

    public int GetCurrentHealth() => currentHealth;
}