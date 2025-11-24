using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EnemyArcher : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation spinePlayer;
    private Rigidbody2D rigid;

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

        InvokeRepeating(nameof(Think), 1f, thinkInterval);
    }

    void Update()
    {
        if (player == null) return;

        // 공격 중에는 모든 환경 변화 체크를 무시하고 현재 상태 유지
        if (isAttacking)
        {
            isActiveAI = false; 
            rigid.linearVelocity = Vector2.zero; 
            return;
        }

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        // Y축 거리 제한 
        if (distY > 1.0f) 
        {
            isActiveAI = true; 
            isStopping = false;
            if (spinePlayer != null) nextMove = (int)Mathf.Sign(spinePlayer.skeleton.ScaleX) * -1; 
            return;
        }

        if (distX <= detectRange)
        {
            //공격
            isActiveAI = false; 
            rigid.linearVelocity = Vector2.zero; 

            LookAtPlayer();
            Attack();
        }
        else
        {
            // 평상시 이동 
            isActiveAI = true;
            isStopping = false; 

            if (spinePlayer != null) nextMove = (int)Mathf.Sign(spinePlayer.skeleton.ScaleX) * -1; 
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

        // X축 반전 
        if (nextMove != 0 && spinePlayer != null)
        {
            spinePlayer.skeleton.ScaleX = nextMove * -1; 
        }

        // 낭떠러지 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Ground"));

        if (!isStopping && rayHit.collider == null)
        {
            StartCoroutine(StopAndTurn());
        }
    }

    void Think()
    {
        if (!isActiveAI || isStopping) return;
        int[] moves = { -1, 1 };
        nextMove = moves[Random.Range(0, moves.Length)];
    }

    IEnumerator StopAndTurn()
    {
        isStopping = true;
        SetAnim("idle");
        rigid.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        nextMove *= -1;
        isStopping = false;
    }

    void LookAtPlayer()
    {
        float dir = player.position.x - transform.position.x;

        // X축 반전 
        spinePlayer.skeleton.ScaleX = Mathf.Sign(dir) * -1;

        // 방향과 반대로 저장 (걷는 방향)
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
            Debug.LogError("Arrow Prefab이 설정되지 않았습니다!");
            return;
        }

        float direction = spinePlayer.skeleton.ScaleX; 
        Vector3 firePosition = transform.position;
        firePosition.y += arrowOffsetY; 

        GameObject arrow = Instantiate(arrowPrefab, firePosition, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // 화살 속도 설정 (몬스터가 바라보는 방향과 반대로 다시 반전시켜 실제 정방향으로)
        rb.linearVelocity = new Vector2(direction * arrowSpeed * -1, 0f); 

        // 화살 방향 설정 (몬스터 방향과 반대로 다시 반전시켜 실제 정방향으로)
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

    
}