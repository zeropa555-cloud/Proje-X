using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;

    public float chaseRange = 12f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    public int damage = 15;

    private float lastAttack;
    private bool isDead = false;

    [HideInInspector]
    public bool canDealDamage = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent < NavMeshAgent > ();
        anim = GetComponent < Animator > ();
    }

    void Update()
    {
        if (isDead || player == null || agent == null) return;

        float d = Vector3.Distance(transform.position, player.position);

        if (d <= attackRange)
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);

            if (Time.time >= lastAttack + attackCooldown)
            {
                Attack();
            }
        }
        else if (d <= chaseRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.SetBool("IsWalking", true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
        }
    }

    void Attack()
    {
        lastAttack = Time.time;
        anim.SetTrigger("Attack");

        // ⭐ OTOMATİK DAR PENCERE (animasyon event gerekmez!)
        StartCoroutine(DamageWindow());
    }

    IEnumerator DamageWindow()
    {
        // 1. Silah kalksın (hasar KAPALI)
        yield return new WaitForSeconds(0.35f);

        // 2. VURUŞ ANI - Sadece 0.2 saniye hasar açık!
        canDealDamage = true;
        Debug.Log("🟢 Vuruş anı! Hasar açık (0.2 sn)");

        yield return new WaitForSeconds(0.2f);

        // 3. Hasar KAPANDI - Artık "havada" vuramaz!
        canDealDamage = false;
        Debug.Log("🔴 Vuruş bitti. Hasar kapalı.");
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent < Collider > ().enabled = false;
    }
}