using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI2 : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;
    private EnemyHealth health;

    [Header("Takip ve Saldırı")]
    public float chaseRange = 20f;      // Daha uzaktan fark eder
    public float attackRange = 3f;       // Biraz daha uzun menzil
    public float attackCooldown = 1.2f; // Daha hızlı saldırır
    public int damage = 20;              // Daha güçlü vurur

    [Header("Hareket Hızları")]
    public float walkSpeed = 4f;
    public float runSpeed = 9f;

    [Header("Rage Modu (Can Azalınca)")]
    public int rageThreshold = 25;       // Can 25'in altına düşünce delirir
    public float rageAttackSpeed = 0.7f; // Rage'de daha hızlı vurur
    public float rageRunSpeed = 12f;     // Rage'de daha hızlı koşar
    public int rageDamage = 30;          // Rage'de daha çok hasar

    private float lastAttack;
    private bool isDead = false;
    private bool isRage = false;

    [HideInInspector]
    public bool canDealDamage = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent < NavMeshAgent > ();
        anim = GetComponent < Animator > ();
        health = GetComponent < EnemyHealth > ();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // RAGE MODU KONTROLÜ - Can azalınca agresifleş!
        if (!isRage && health != null && health.GetCurrentHealth() <= rageThreshold)
        {
            EnterRage();
        }

        // Saldırı mesafesindeyse
        if (distance <= attackRange)
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", false);

            if (Time.time >= lastAttack + attackCooldown)
            {
                Attack();
            }
        }
        // Takip mesafesindeyse
        else if (distance <= chaseRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // Uzaktaysa KOŞ, yakındaysa YÜRÜ
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
        // Çok uzaktaysa bekle
        else
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", false);
        }
    }

    void EnterRage()
    {
        isRage = true;
        attackCooldown = rageAttackSpeed;
        runSpeed = rageRunSpeed;
        damage = rageDamage;
        anim.SetTrigger("Hit"); // Rage animasyonu tetikler (opsiyonel)
        Debug.Log("👹 Düşman RAGE moduna girdi! Daha hızlı ve güçlü!");
    }

    void Attack()
    {
        lastAttack = Time.time;
        anim.SetTrigger("Attack");
        StartCoroutine(AutoDamageWindow());
    }

    IEnumerator AutoDamageWindow()
    {
        yield return new WaitForSeconds(0.25f);
        canDealDamage = true;
        yield return new WaitForSeconds(0.4f);
        canDealDamage = false;
    }

    // Animasyon Event olarak da kullanılabilir
    public void EnableWeaponDamage() => canDealDamage = true;
    public void DisableWeaponDamage() => canDealDamage = false;

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent < Collider > ().enabled = false;
    }
}