using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    private Animator anim;
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent < Animator > ();
        enemyAI = GetComponent < EnemyAI > ();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Ölüyken hasar almasýn

        currentHealth -= damage;

        // Hasar alma animasyonu
        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        enemyAI?.Die(); // AI'yi kapat
        anim.SetTrigger("Die"); // Ölüm animasyonu
        Destroy(gameObject, 3f); // 3 saniye sonra yok ol
    }
}