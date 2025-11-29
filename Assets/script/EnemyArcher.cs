using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EnemyArcher : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation spinePlayer;
    [HideInInspector] private Rigidbody2D rigid;

    // == 기본 이동 설정 ==
    [Header("== 기본 이동 설정 ==")]
    public float speed = 1f;
    public int nextMove = 1;
    protected bool isStopping = false;
    protected string currentAnim = "";
    private float thinkInterval = 3f;

    // == 아처 특수 설정 ==
    [Header("== Archer Settings ==")]
    public float detectRange = 6f;
    public float attackCooldown = 2f;

    // Y축 포함된 화살 설정
    [Header("== Arrow Settings ==")]
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float arrowOffsetY = 0.5f;

    private Transform player;
    private bool isAttacking = false;
    private bool isActiveAI = true;

    // 코루틴 제어 변수
    private Coroutine stopAndTurnCoroutine;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

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
        if (player == null) return;

        // 공격 중이면 AI 정지
        if (isAttacking)
        {
            isActiveAI = false;
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        // 높이 차이가 클 때 이동만 유지
        if (distY > 1.0f)
        {
            isActiveAI = true;

            if (spinePlayer != null)
                nextMove = (int)Mathf.Sign(spinePlayer.skeleton.ScaleX) * -1;

            return;
        }

        // 공격 범위 안
        if (distX <= detectRange)
        {
            isActiveAI = false;
            rigid.linearVelocity = Vector2.zero;

            LookAtPlayer();
            Attack();
        }
        else
        {
            // 이동 모드 ON
            isActiveAI = true;

            if (spinePlayer != null)
                nextMove = (int)Mathf.Sign(spinePlayer.skeleton.ScaleX) * -1;
        }
    }

    void FixedUpdate()
    {
        if (isAttacking || !isActiveAI) return;

        if (!isStopping)
        {
            rigid.linearVelocity = new Vector2(nextMove * speed, rigid.linearVelocity.y);
            if (nextMove != 0) SetAnim("walk");
        }
        else
        {
            rigid.linearVelocity = Vector2.zero;
            SetAnim("idle");
        }

        // 방향 보정
        if (nextMove != 0 && spinePlayer != null)
        {
            spinePlayer.skeleton.ScaleX = nextMove * -1;
        }

        // 레이 감지는 코루틴 중일 때 OFF
        if (stopAndTurnCoroutine == null)
        {
            Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Ground"));

            if (isActiveAI && rayHit.collider == null)
            {
                isStopping = true;
                stopAndTurnCoroutine = StartCoroutine(StopAndTurn());
            }
        }
    }

    /* void Think()
    {
        if (!isActiveAI || isStopping) return;
        int[] moves = { -1, 1 };
        nextMove = moves[Random.Range(0, moves.Length)];
    }*/

    IEnumerator StopAndTurn()
    {
        rigid.linearVelocity = Vector2.zero;
        SetAnim("idle");

        yield return new WaitForSeconds(1f);

        nextMove *= -1;

        isStopping = false;
        stopAndTurnCoroutine = null;
    }

    // ===== 공격 관련 =====

    void LookAtPlayer()
    {
        float dir = player.position.x - transform.position.x;
        spinePlayer.skeleton.ScaleX = Mathf.Sign(dir) * -1;
        nextMove = (int)Mathf.Sign(dir);
    }

    void Attack()
    {
        if (spinePlayer == null) return;
        if (!isAttacking)
        {
            isAttacking = true;
            SetAnim("attack", false);
        }
    }

    void OnAnimComplete(Spine.TrackEntry track)
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
        if (arrowPrefab == null)
        {
            
            return;
        }

        float direction = spinePlayer.skeleton.ScaleX;

        Vector3 firePosition = transform.position;
        firePosition.y += arrowOffsetY;

        GameObject arrow = Instantiate(arrowPrefab, firePosition, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        rb.linearVelocity = new Vector2(direction * arrowSpeed * -1, 0f);

        arrow.transform.localScale = new Vector3(direction * -1, 1, 1);
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void SetAnim(string animName, bool loop = true)
    {
        if (spinePlayer && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
            currentAnim = animName;
        }
    }

    private void OnDrawGizmos()
    {
        if (rigid == null) return;
        if (stopAndTurnCoroutine != null) return;

        Gizmos.color = Color.red;

        Vector2 start = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Vector2 end = start + Vector2.down * 1f;

        Gizmos.DrawLine(start, end);
    }
}
