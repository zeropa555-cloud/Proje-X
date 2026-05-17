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
        Debug.Log("👹 Düşman canı: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("💔 EnemyHealth.TakeDamage çağrıldı! Hasar: " + damage);

        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("👹 Kalan can: " + currentHealth);

        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 Düşman öldü!");
        enemyAI?.Die();
        anim.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }
}