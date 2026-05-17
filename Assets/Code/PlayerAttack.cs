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

        // Animasyon süresini al
        float animLength = swordAnimator.GetCurrentAnimatorStateInfo(0).length;

        // 1. Animasyonun baþýnda hasar KAPALI (zaten kapalý)
        yield return new WaitForSeconds(animLength * 0.25f); // Kýlýç kalkana kadar bekle

        // 2. VURUÞ ANI - Hasar AÇILIR
        if (swordDamage != null)
            swordDamage.EnableDamage();

        // 3. Vuruþ penceresi (kýlýç düþmana deðecek kadar süre)
        yield return new WaitForSeconds(animLength * 0.3f);

        // 4. Hasar KAPANIR - Artýk vuramaz
        if (swordDamage != null)
            swordDamage.DisableDamage();

        // 5. Animasyonun bitmesini bekle
        yield return new WaitForSeconds(animLength * 0.45f);

        swordAnimator.ResetTrigger("Attack");
        isAttacking = false;
    }
}