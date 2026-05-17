using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 25;
    public float hitCooldown = 0.5f;
    private float lastHit = -999f;

    [HideInInspector]
    public bool canDealDamage = false;

    public void EnableDamage()
    {
        canDealDamage = true;
        Debug.Log("🟢 EnableDamage() çağrıldı");
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        Debug.Log("🔴 DisableDamage() çağrıldı");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("⚔️ Kılıç bir şeye değdi: " + other.name + " | Tag: " + other.tag);

        if (!canDealDamage)
        {
            Debug.Log("⛔ canDealDamage FALSE! Hasar verilmiyor.");
            return;
        }

        if (!other.CompareTag("Enemy"))
        {
            Debug.Log("❌ Enemy tag'i yok, bu: " + other.tag);
            return;
        }

        if (Time.time < lastHit + hitCooldown)
        {
            Debug.Log("⏳ Cooldown aktif");
            return;
        }

        // DÜŞMANIN ANA OBJESİNDE EnemyHealth ara (child'larda da bakar)
        EnemyHealth eh = other.GetComponent<EnemyHealth>();
        if (eh == null) eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null) eh = other.GetComponentInChildren<EnemyHealth>();

        if (eh != null)
        {
            eh.TakeDamage(damage);
            lastHit = Time.time;
            Debug.Log("💥 BAŞARILI! Hasar verildi: " + damage);
        }
        else
        {
            Debug.LogError("❌ EnemyHealth bulunamadı! " + other.name + " ve üst/alt objelerinde yok!");
        }
    }
}