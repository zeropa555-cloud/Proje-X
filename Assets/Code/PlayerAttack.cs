using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator swordAnimator;
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

        yield return new WaitForSeconds(swordAnimator.GetCurrentAnimatorStateInfo(0).length);

        swordAnimator.ResetTrigger("Attack");
        isAttacking = false;
    }
}