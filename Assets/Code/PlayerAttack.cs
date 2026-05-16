using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator swordAnimator; // Inspector'dan kýlýcýn Animator'unu sürükle

    private bool isActionPlaying = false; // Spam korumasý

    void Update()
    {
        // Sol týk = Saldýrý (Mouse 0)
        if (Input.GetMouseButtonDown(0) && !isActionPlaying)
        {
            StartCoroutine(PlayAction("Attack"));
        }

        // Sað týk = Blok / Savunma (Mouse 1)
        if (Input.GetMouseButtonDown(1) && !isActionPlaying)
        {
            StartCoroutine(PlayAction("Block"));
        }
    }

    IEnumerator PlayAction(string triggerName)
    {
        isActionPlaying = true;
        swordAnimator.SetTrigger(triggerName);

        // Animasyon bitene kadar bekle
        yield return new WaitForSeconds(swordAnimator.GetCurrentAnimatorStateInfo(0).length);

        swordAnimator.ResetTrigger(triggerName);
        isActionPlaying = false;
    }
}