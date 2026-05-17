using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 25;
    public float hitCooldown = 0.5f; // 0.5 saniyede 1 vurur

    private float lastHitTime = -999f;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("⚔️ Kılıç değdi: " + other.name + " | Tag: " + other.tag);

        // Sadece Enemy tag'ine sahip objelere vur
        if (!other.CompareTag("Enemy"))
        {
            Debug.Log("❌ Enemy değil");
            return;
        }

        // Cooldown - çok hızlı vurmasın
        if (Time.time < lastHitTime + hitCooldown)
        {
            Debug.Log("⏳ Cooldown aktif");
            return;
        }

        // Hasar ver
        EnemyHealth eh = other.GetComponent < EnemyHealth > ();
        if (eh != null)
        {
            eh.TakeDamage(damage);
            lastHitTime = Time.time;
            Debug.Log("💥 DÜŞMANA HASAR VERİLDİ: " + damage);
        }
        else
        {
            Debug.LogError("❌ EnemyHealth yok! Düşmanın ana objesinde mi?");
        }
    }
}