using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI2 : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;
    private EnemyHealth health;

    public float chaseRange = 20f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public int damage = 20;

    public float walkSpeed = 4f;
    public float runSpeed = 9f;

    public int rageThreshold = 25;
    public float rageAttackSpeed = 0.8f;
    public float rageRunSpeed = 12f;
    public int rageDamage = 30;

    private float lastAttack = -999f;
    private bool isDead = false;
    private bool isRage = false;

    [HideInInspector]
    public bool canDealDamage = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isRage && health != null && health.GetCurrentHealth() <= rageThreshold)
        {
            EnterRage();
        }

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", false);

            if (Time.time >= lastAttack + attackCooldown)
            {
                lastAttack = Time.time;
                anim.ResetTrigger("Attack");
                anim.SetTrigger("Attack");
                StartCoroutine(DamageWindow());
            }
        }
        else if (distance <= chaseRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (distance > 6f)
            {
                agent.speed = runSpeed;
                anim.SetBool("IsRunning", true);
                anim.SetBool("IsWalking", false);
            }
            else
            {
                agent.speed = walkSpeed;
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsRunning", false);
            }
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", false);
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

    void EnterRage()
    {
        isRage = true;
        attackCooldown = rageAttackSpeed;
        runSpeed = rageRunSpeed;
        damage = rageDamage;
        anim.SetTrigger("Hit");
        Debug.Log("👹 RAGE MODU!");
    }

    IEnumerator DamageWindow()
    {
        yield return new WaitForSeconds(0.25f);
        canDealDamage = true;
        yield return new WaitForSeconds(0.35f);
        canDealDamage = false;
    }

    public void EnableWeaponDamage() => canDealDamage = true;
    public void DisableWeaponDamage() => canDealDamage = false;

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
    }
}