using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 25;
    public float hitCooldown = 0.5f;
    private float lastHit = -999f;

    private bool canDealDamage = false; // BAŞTA KAPALI!

    // PlayerCombat'tan çağrılacak - Animasyonun vurduğu anında açılır
    public void EnableDamage()
    {
        canDealDamage = true;
        Debug.Log("🟢 Kılıç hasar AÇIK");
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        Debug.Log("🔴 Kılıç hasar KAPALI");
    }

    void OnTriggerEnter(Collider other)
    {
        // ⛔ Animasyon oynamıyorsa hasar YOK!
        if (!canDealDamage) return;

        if (!other.CompareTag("Enemy")) return;
        if (Time.time < lastHit + hitCooldown) return;

        EnemyHealth eh = other.GetComponent < EnemyHealth > ();
        if (eh != null)
        {
            eh.TakeDamage(damage);
            lastHit = Time.time;
            Debug.Log("⚔️ Hasar verildi: " + damage);
        }
    }
}