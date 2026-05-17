using System.Collections;
using UnityEngine;

// Bu satir, kodu attigin objeye otomatik olarak Rigidbody ekler (unutmani engeller)
[RequireComponent(typeof(Rigidbody))] 
public class GladiatorBoss : MonoBehaviour
{
    public enum BossState { Chasing, Attacking, Charging, Cooldown }
    public BossState currentState = BossState.Chasing;

    [Header("References")]
    public Transform player;
    private Rigidbody rb;
    
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float chargeSpeed = 12f;
    public float attackRange = 2f;

    [Header("Timers & Cooldowns")]
    public float chargeCooldown = 5f;
    public float chargeDuration = 1f;
    public float attackRate = 1.5f;

    private float nextChargeTime;
    private float nextAttackTime;
    private Vector3 chargeDirection;
    private bool isCharging = false;

    void Start()
    {
        // Rigidbody ayarlarini kod üzerinden otomatik yapiyoruz
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Carpinca yere dusup yuvarlanmasin
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Hareketin titremesini onler

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        
        nextChargeTime = Time.time + chargeCooldown;
    }

    // Girdiler ve zamanlayicilar Update icinde kontrol edilir
    void Update()
    {
        if (player == null) return;

        if (currentState == BossState.Chasing)
        {
            LookAtTarget(player.position);
            
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                currentState = BossState.Attacking;
            }
            else if (Time.time >= nextChargeTime)
            {
                StartCoroutine(PerformCharge());
            }
        }
        else if (currentState == BossState.Attacking)
        {
            LookAtTarget(player.position);
            
            if (Vector3.Distance(transform.position, player.position) > attackRange)
            {
                currentState = BossState.Chasing;
            }
            else if (Time.time >= nextAttackTime)
            {
                Debug.Log("Boss Normal Saldırı Yaptı!"); 
                nextAttackTime = Time.time + attackRate;
            }
        }
    }

    // Fiziksel hareketleri (duvarlardan gecmemesi icin) FixedUpdate icinde yapiyoruz
    void FixedUpdate()
    {
        if (player == null) return;

        if (currentState == BossState.Chasing)
        {
            // rb.MovePosition fizik motorunu kullanarak yurumesini saglar
            Vector3 targetPosition = Vector3.MoveTowards(rb.position, player.position, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
        else if (currentState == BossState.Charging && isCharging)
        {
            // Hucum ederken de fizik motorunu kullaniyoruz
            Vector3 targetPosition = rb.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }

    IEnumerator PerformCharge()
    {
        currentState = BossState.Charging;
        rb.linearVelocity = Vector3.zero; // Hucum oncesi kaymayi durdur
        
        Vector3 targetPos = player.position;
        LookAtTarget(targetPos);
        chargeDirection = (targetPos - transform.position).normalized;
        chargeDirection.y = 0; // Havaya dogru ucmasini engeller
        
        yield return new WaitForSeconds(0.5f); 

        isCharging = true; // FixedUpdate icindeki hucum hareketini baslat
        yield return new WaitForSeconds(chargeDuration);
        isCharging = false; // Hucum hareketini durdur

        currentState = BossState.Cooldown;
        rb.linearVelocity = Vector3.zero; // Carptiktan sonra sekip savrulmasini engeller
        
        yield return new WaitForSeconds(1.5f); 

        currentState = BossState.Chasing;
        nextChargeTime = Time.time + chargeCooldown;
    }

    void LookAtTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0; 
        if (direction != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}