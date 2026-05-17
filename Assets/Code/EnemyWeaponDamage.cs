using UnityEngine;

public class EnemyWeaponDamage : MonoBehaviour
{
    public int damage = 15;
    public float hitCooldown = 1f;
    private float lastHit = -999f;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ⭐ GERÇEK MESAFE KONTROLÜ - Silah değse bile uzaktaysa vurma!
        // Düşmanın ana objesi (root) ile Player arası mesafe
        float realDistance = Vector3.Distance(transform.root.position, other.transform.position);

        // attackRange'i EnemyAI2'den oku
        var ai2 = GetComponentInParent < EnemyAI2 > ();
        float maxRange = (ai2 != null) ? ai2.attackRange : 3f;

        // Mesafe fazlaysa hasar verme! (0.3f tolerans)
        if (realDistance > maxRange + 0.3f)
        {
            Debug.Log("⛔ Silah değiyor ama mesafe fazla! Gerçek mesafe: " + realDistance.ToString("F1"));
            return;
        }

        // canDealDamage kontrolü
        bool canHit = false;
        var ai1 = GetComponentInParent < EnemyAI > ();
        if (ai1 != null && ai1.canDealDamage) canHit = true;
        if (ai2 != null && ai2.canDealDamage) canHit = true;

        if (!canHit) return;

        // Cooldown kontrolü
        if (Time.time < lastHit + hitCooldown) return;

        // Hasar ver
        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
            lastHit = Time.time;
            Debug.Log("💥 VURDU! Mesafe: " + realDistance.ToString("F1") + " | Hasar: " + damage);
        }
    }
}