using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;

public class EnemyMelee : EnemyMove
{
    [Header("== 근접 공격 설정 ==")]
    public float chaseRange = 6.0f;
    public float attackRange = 0.7f;
    public float attackCooldown = 1.5f;

    [Header("== 근접 피해 설정 ==")]
    public GameObject normalDamageObj;
    public float damageDuration = 0.4f;

    protected Transform player;
    protected bool isAttacking = false;
    protected bool isCoolingDown = false;

    protected override void Awake()
    {
        base.Awake();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (normalDamageObj != null)
            normalDamageObj.SetActive(false);

        if (spinePlayer != null)
            spinePlayer.AnimationState.Complete += OnAnimComplete;

        int[] moves = { -1, 1 };
        nextMove = moves[Random.Range(0, moves.Length)];
    }

    void Update()
    {
        if (player == null) return;
        if (isAttacking || isCoolingDown) return;

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        if (distY > 1.0f)
        {
            isActiveAI = true;
            return;
        }

        // 공격
        if (distX <= attackRange)
        {
            isActiveAI = false;
            rigid.linearVelocity = Vector2.zero;
            LookAtPlayer();
            Attack();
        }
        // 추격
        else if (distX <= chaseRange)
        {
            isActiveAI = true;
            LookAtPlayer();
        }
        else
        {
            isActiveAI = true;
        }
    }

    protected virtual void LookAtPlayer()
    {
        float dir = player.position.x - transform.position.x;
        int s = (int)Mathf.Sign(dir);

        nextMove = s;

        if (normalDamageObj != null)
        {
            Vector3 p = normalDamageObj.transform.localPosition;
            p.x = s * 1.4f;
            normalDamageObj.transform.localPosition = p;
        }
    }

    protected virtual void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        SetAnim("attack", false);

        StartCoroutine(ActivateDamage());
    }

    protected void OnAnimComplete(TrackEntry track)
    {
        string anim = track.Animation.Name;

    if (anim == "attack" || anim == "skill")
    {
        isAttacking = false;
        StartCoroutine(AttackDelay());
        SetAnim("idle");
    } 
    }

    protected IEnumerator ActivateDamage()
    {
        yield return new WaitForSeconds(0.3f);

        if (normalDamageObj != null)
        {
            normalDamageObj.SetActive(true);
            yield return new WaitForSeconds(damageDuration);
            normalDamageObj.SetActive(false);
        }
    }

    IEnumerator AttackDelay()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(attackCooldown);
        isCoolingDown = false;
    }

    protected virtual void OnDestroy()
    {
        if (spinePlayer != null)
            spinePlayer.AnimationState.Complete -= OnAnimComplete;
    }
}
