using UnityEngine;

public class EnemyWeaponDamage : MonoBehaviour
{
    public int damage = 15;
    public float hitCooldown = 1f;
    private float lastHit = -999f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < lastHit + hitCooldown) return;

        // Üst objede EnemyAI veya EnemyAI2 ara
        bool canHit = false;

        EnemyAI ai1 = GetComponentInParent < EnemyAI > ();
        EnemyAI2 ai2 = GetComponentInParent < EnemyAI2 > ();

        if (ai1 != null && ai1.canDealDamage) canHit = true;
        if (ai2 != null && ai2.canDealDamage) canHit = true;

        if (!canHit) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
            lastHit = Time.time;
            Debug.Log("💥 Düşman vurdu!");
        }
    }
}