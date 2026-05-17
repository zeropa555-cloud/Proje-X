using UnityEngine;

public class EnemyWeaponDamage : MonoBehaviour
{
    public int damage = 15;
    public EnemyAI enemyAI; // Inspector'dan düşmanın ana objesini sürükle!

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("⚔️ Silah değdi: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("🎯 Player bulundu!");

            if (enemyAI == null)
            {
                Debug.LogError("❌ enemyAI NULL! Inspector'dan düşmanı bağlamadın!");
                return;
            }

            Debug.Log("🔍 canDealDamage değeri: " + enemyAI.canDealDamage);

            if (enemyAI.canDealDamage)
            {
                PlayerHealth ph = other.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(damage);
                    Debug.Log("💥 HASAR VERİLDİ: " + damage);
                }
                else
                {
                    Debug.LogError("❌ PlayerHealth bulunamadı! Player objesinde yok!");
                }
            }
            else
            {
                Debug.Log("⛔ canDealDamage FALSE! Animasyon event eklenmemiş olabilir.");
            }
        }
    }
}