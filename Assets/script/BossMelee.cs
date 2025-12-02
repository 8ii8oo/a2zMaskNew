using UnityEngine;
using System.Collections;

public class BossMelee : EnemyMelee
{
    [Header("보스 전용 공격 ")]
    public GameObject skillDamageObj;
    public int hitsBeforeSkill = 4;

    private int attackCount = 0;

    protected override void Awake()
    {
        base.Awake();

        if (skillDamageObj != null)
            skillDamageObj.SetActive(false);
    }

    protected override void LookAtPlayer()
    {

        base.LookAtPlayer();

        int s = nextMove;

        if (skillDamageObj != null)
        {
            Vector3 p = skillDamageObj.transform.localPosition;
            p.x = s * 1.4f; 
            skillDamageObj.transform.localPosition = p;
        }
    }

    protected override void Attack()
    {
        if (isAttacking) return;

        attackCount++;

        if (attackCount >= hitsBeforeSkill)
        {
            attackCount = 0;
            SkillAttack();
        }
        else
        {
            NormalAttack();
        }
    }

    private void NormalAttack()
    {
        isAttacking = true;
        SetAnim("attack", false);
        StartCoroutine(ActivateDamage());
    }

    private void SkillAttack()
    {
        isAttacking = true;
        SetAnim("skill", false);
        StartCoroutine(ActivateSkillDamage());
    }

    IEnumerator ActivateSkillDamage()
    {
        yield return new WaitForSeconds(0.8f);

        if (skillDamageObj != null)
        {
            skillDamageObj.SetActive(true);
            yield return new WaitForSeconds(damageDuration + 0.5f);
            skillDamageObj.SetActive(false);
        }
    }
}