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
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        enemyAI?.Die();
        anim.SetTrigger("Die");

        // DÜŞMAN ÖLÜNCE OTOMATİK SAHNE DEĞİŞİMİ!
        Debug.Log("☠️ Düşman öldü! Yeni sahneye geçiliyor...");
        GameManager.Instance.GoToNextLevel();

        Destroy(gameObject, 3f);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}