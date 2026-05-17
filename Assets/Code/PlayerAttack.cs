using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator swordAnimator;
    public SwordDamage swordDamage; // Inspector'dan KILIÇ objesini sürükle!
    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PlayAttack());
        }
    }

    IEnumerator PlayAttack()
    {
        isAttacking = true;
        swordAnimator.SetTrigger("Attack");

        // 1. Kılıç kalkana kadar bekle (sabit 0.3 saniye)
        yield return new WaitForSeconds(0.3f);

        // 2. HASAR AÇIK (sabit 0.4 saniye)
        if (swordDamage != null)
        {
            swordDamage.EnableDamage();
            Debug.Log("🟢 Kılıç hasar AÇIK");
        }

        yield return new WaitForSeconds(0.4f);

        // 3. HASAR KAPALI
        if (swordDamage != null)
        {
            swordDamage.DisableDamage();
            Debug.Log("🔴 Kılıç hasar KAPALI");
        }

        // 4. Animasyon bitene kadar bekle (toplam 1 saniye olsun)
        yield return new WaitForSeconds(0.3f);

        swordAnimator.ResetTrigger("Attack");
        isAttacking = false;
        Debug.Log("✅ Saldırı bitti");
    }
}