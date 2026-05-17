using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;

    public float chaseRange = 12f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float damageWindowDuration = 0.5f; // Hasar verebileceği süre (saniye)

    private float lastAttackTime;
    private bool isDead = false;

    [HideInInspector]
    public bool canDealDamage = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        agent = GetComponent < NavMeshAgent > ();
        anim = GetComponent < Animator > ();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else if (distance <= chaseRange)
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
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");
        StartCoroutine(AutoDamageWindow());
    }

    // Animasyon event yerine otomatik hasar penceresi (güvenlik)
    IEnumerator AutoDamageWindow()
    {
        yield return new WaitForSeconds(0.2f); // Saldırı başlasın diye bekle
        canDealDamage = true;
        Debug.Log("🟢 Düşman hasar verebilir!");

        yield return new WaitForSeconds(damageWindowDuration);
        canDealDamage = false;
        Debug.Log("🔴 Düşman hasar veremez!");
    }

    // BU 2 METODU YİNE DE ANİMASYON EVENT OLARAK EKLERSEN DAHA KESİN OLUR:
    public void EnableWeaponDamage()
    {
        canDealDamage = true;
        Debug.Log("🟢 Event: EnableWeaponDamage");
    }

    public void DisableWeaponDamage()
    {
        canDealDamage = false;
        Debug.Log("🔴 Event: DisableWeaponDamage");
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        GetComponent < Collider > ().enabled = false;
    }
}