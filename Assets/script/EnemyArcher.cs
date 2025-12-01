using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine;

public class EnemyArcher : EnemyMove
{
    [Header("아처세팅")]
    public float detectRange = 6f;       // 플레이어 감지 범위
    public float attackCooldown = 2f;    // 공격 쿨타임
    
    [Header("화살세팅")]
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float arrowOffsetY = 0.5f;

    private Transform player;
    private bool isAttacking = false;
    private bool isCoolingDown = false;

    protected override void Awake()
    {
        base.Awake();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (spinePlayer != null)
        {
            SetAnim("idle");
            spinePlayer.AnimationState.Complete += OnAnimComplete;
        }

        int[] moves = { -1, 1 };
        nextMove = moves[Random.Range(0, moves.Length)];
    }

    void Update()
    {
        if (player == null || isAttacking || isCoolingDown) return;

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        // Y축 높이 차이가 심하면 이동만
        if (distY > 1.0f)
        {
            isActiveAI = true;
            return;
        }

        // 공격 범위 내
        if (distX <= detectRange)
        {
            isActiveAI = false;
            rigid.linearVelocity = Vector2.zero;
            LookAtPlayer();
            Attack();
        }
        else
        {
            //기본 이동/턴 로직 수행
            isActiveAI = true;
            LookAtPlayerMoveDirection();
        }
    }

    // 방향 보정
    void LookAtPlayerMoveDirection()
    {
        if (player == null) return;

        float dir = player.position.x - transform.position.x;
        nextMove = (int)Mathf.Sign(dir);
    }

    void LookAtPlayer()
    {
        if (player == null || spinePlayer == null) return;

        float dir = player.position.x - transform.position.x;
        float s = Mathf.Sign(dir);

        spinePlayer.skeleton.ScaleX = s * -1;
        nextMove = (int)s;
    }

    //공격 관련

    void Attack()
    {
        if (spinePlayer == null) return;

        if (!isAttacking)
        {
            isAttacking = true;
            SetAnim("attack", false); 
        }
    }

    void OnAnimComplete(TrackEntry track)
    {
        if (track.Animation.Name == "attack")
        {
            ShootArrow();
            StartCoroutine(AttackDelay());
            SetAnim("idle");
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null) return;

        float direction = spinePlayer.skeleton.ScaleX;

        Vector3 firePosition = transform.position;
        firePosition.y += arrowOffsetY;

        GameObject arrow = Instantiate(arrowPrefab, firePosition, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // X 방향 계산
        rb.linearVelocity = new Vector2(direction * arrowSpeed * -1, 0f);

        arrow.transform.localScale = new Vector3(direction * -1, 1, 1);
    }

    IEnumerator AttackDelay()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(attackCooldown);
        isCoolingDown = false;
        isAttacking = false;
    }

    protected void SetAnim(string animName, bool loop = true)
    {
        if (!spinePlayer) return;

        if (spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
        }
    }

    void OnDestroy()
    {
        if (spinePlayer != null)
        {
            spinePlayer.AnimationState.Complete -= OnAnimComplete;
        }
    }
}
