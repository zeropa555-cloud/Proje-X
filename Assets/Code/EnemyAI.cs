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
    public float damageWindow = 0.5f; // Hasar verebileceği süre

    private float lastAttack;
    private bool isDead = false;

    // BU DEĞİŞKEN EKLENDİ!
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
                lastAttack = Time.time;
                anim.SetTrigger("Attack");
                StartCoroutine(AutoDamageWindow()); // Animasyon event unutursan diye güvenlik
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

    // Animasyon event olarak da kullanabilirsin, ama unutursan coroutine devrede
    IEnumerator AutoDamageWindow()
    {
        yield return new WaitForSeconds(0.3f); // Saldırı başlasın
        canDealDamage = true;
        yield return new WaitForSeconds(damageWindow);
        canDealDamage = false;
    }

    // BU METODU ANİMASYON EVENT OLARAK DA EKLEYEBİLİRSİN
    public void EnableWeaponDamage() => canDealDamage = true;
    public void DisableWeaponDamage() => canDealDamage = false;

    // Eski adla da çağrılabilsin (animasyon event uyumluluğu)
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
        GetComponent < Collider > ().enabled = false;
    }
}