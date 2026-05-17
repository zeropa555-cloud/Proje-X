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

    private float lastAttack = -999f;
    private bool isDead = false;

    [HideInInspector]
    public bool canDealDamage = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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
                lastAttack = Time.time;
                anim.SetTrigger("Attack");
                StartCoroutine(DamageWindow());
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

        // ⬇️ YENİ: Düşman her zaman oyuncuya dönsün!
        if (player != null && !isDead)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
            }
        }
    }

    IEnumerator DamageWindow()
    {
        yield return new WaitForSeconds(0.3f);
        canDealDamage = true;
        yield return new WaitForSeconds(0.2f);
        canDealDamage = false;
    }

    public void EnableWeaponDamage() => canDealDamage = true;
    public void DisableWeaponDamage() => canDealDamage = false;

    public void DealDamageToPlayer()
    {
        if (isDead || player == null) return;
        float d = Vector3.Distance(transform.position, player.position);
        if (d <= attackRange + 0.5f)
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
    }
}