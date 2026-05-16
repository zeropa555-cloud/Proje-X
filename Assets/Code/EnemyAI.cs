using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;

    public float chaseRange = 12f;   // Takip mesafesi
    public float attackRange = 2f;   // Saldýrý mesafesi
    public float attackCooldown = 2f; // Saldýrý aralýðý

    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent < NavMeshAgent > ();
        anim = GetComponent < Animator > ();
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 1. Saldýrý mesafesindeyse
        if (distance <= attackRange)
        {
            agent.isStopped = true; // Dur
            anim.SetBool("IsWalking", false);

            // Saldýrý cooldown'ý geçtiyse vur
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        // 2. Takip mesafesindeyse
        else if (distance <= chaseRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position); // Player'a git
            anim.SetBool("IsWalking", true); // Yürüme animasyonu
        }
        // 3. Çok uzaktaysa
        else
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack"); // Saldýrý animasyonu
    }

    // BU METODU DÜÞMAN SALDIRI ANIMASYONUNUN ORTASINA EVENT OLARAK EKLE!
    public void DealDamageToPlayer()
    {
        if (player != null && !isDead)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(15);
        }
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent < Collider > ().enabled = false; // Çarpýþmayý kapat
    }
}