using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EnemyArcher : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation spinePlayer;
    private Rigidbody2D rigid;
    private Collider2D enemyCollider; // 낭떠러지 체크용 Collider2D 참조

    [Header("이동설정")]
    public float speed = 1f;
    public int nextMove = 1; // 기본값 1 -> 처음부터 오른쪽으로 이동
    protected bool isStopping = false;
    protected string currentAnim = "";

    [Header("아처세팅")]
    public float detectRange = 6f;
    public float attackCooldown = 2f;

    [Header("화살세팅")]
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float arrowOffsetY = 0.5f;

    private Transform player;
    private bool isAttacking = false;
    private bool isActiveAI = true;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>(); // Collider2D 참조 가져오기

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (spinePlayer != null)
        {
            SetAnim("idle");
            spinePlayer.AnimationState.Complete += OnAnimComplete;
        }

        // nextMove가 0이면 1로 초기화 (Awake에서 이미 nextMove=1로 설정되어 있으나 안전 장치)
        if (nextMove == 0) nextMove = 1;
    }

    void Update()
    {
        if (player == null) return;

        // 1. 공격 중이면 모든 AI 및 이동 중지
        if (isAttacking)
        {
            isActiveAI = false;
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        // 2. 높이 차가 크면 (순찰 모드 유지)
        if (distY > 1.0f)
        {
            isActiveAI = true;
            isStopping = false;
            // nextMove는 낭떠러지 로직에게 맡기고 여기서 덮어쓰지 않음
            return;
        }

        // 3. 플레이어가 탐지 범위 안이면 (공격 상태로 전환)
        if (distX <= detectRange)
        {
            isActiveAI = false;
            // 이동 멈춤
            rigid.linearVelocity = Vector2.zero;
            isStopping = false; // 공격 중에는 낭떠러지 정지 상태 해제

            LookAtPlayer();
            Attack();
        }
        else
        {
            // 4. 플레이어 멀리 있으면 (순찰 모드 재개)
            isActiveAI = true;
            isStopping = false;
            // nextMove는 낭떠러지 로직에게 맡기고 여기서 덮어쓰지 않음
        }
    }

    void FixedUpdate()
    {
        // 낭떠러지 체크는 모든 상태에서 실행
        CheckCliff();

        // 공격 중이거나 AI 비활성화이면 이동 중단
        if (isAttacking || !isActiveAI) 
        {
            rigid.linearVelocity = Vector2.zero;
            SetAnim("idle");
            return;
        }

        // 실제 이동 처리
        if (!isStopping)
        {
            rigid.linearVelocity = new Vector2(nextMove * speed, rigid.linearVelocity.y);
            if (nextMove != 0) SetAnim("walk");
        }
        else
        {
            // isStopping일 때는 rigid.linearVelocity를 0으로 유지
            rigid.linearVelocity = new Vector2(0f, rigid.linearVelocity.y);
            SetAnim("idle");
        }

        // Spine 좌우 반전
        if (nextMove != 0 && spinePlayer != null)
        {
            spinePlayer.skeleton.ScaleX = nextMove * -1;
        }
    }

    void CheckCliff()
    {
        if (isStopping || enemyCollider == null) return;

        // 몬스터 발바닥 Y 위치 (콜라이더 바닥)
        float footY = rigid.position.y - enemyCollider.bounds.extents.y;
        
        // 낭떠러지 체크를 위한 X 오프셋 (몬스터 발가락 바로 앞)
        float cliffCheckOffsetX = enemyCollider.bounds.extents.x + 0.1f; 
        
        // Raycast 시작 위치: 몬스터 이동 방향 앞쪽, 발바닥 높이
        Vector2 startPos = new Vector2(
            rigid.position.x + nextMove * cliffCheckOffsetX, 
            footY 
        );
        
        // Raycast 길이: 땅을 확실히 감지하도록 0.5f로 설정
        float rayLength = 0.5f; 

        // Raycast 발사 (Ground 레이어만)
        RaycastHit2D rayHit = Physics2D.Raycast(startPos, Vector2.down, rayLength, LayerMask.GetMask("Ground"));

        // Debug.DrawRay(startPos, Vector2.down * rayLength, rayHit.collider != null ? Color.green : Color.red);
        
        if (rayHit.collider == null)
        {
            StartCoroutine(StopAndTurn());
        }
    }

    IEnumerator StopAndTurn()
    {
        // 중복 호출 방지 및 상태 전환
        isStopping = true;
        SetAnim("idle");
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        nextMove *= -1;
        isStopping = false;
    }

    void LookAtPlayer()
    {
        if (player == null || spinePlayer == null) return;

        float dir = player.position.x - transform.position.x;
        float s = Mathf.Sign(dir);

        // Spine 방향 세팅
        spinePlayer.skeleton.ScaleX = s * -1;

        // nextMove를 플레이어 방향으로 설정 (공격이 끝나고 순찰을 재개할 때 사용됨)
        nextMove = (int)s;
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
        if (rb != null)
        {
            // 화살 속도 설정 (Spine 방향과 반대로)
            rb.linearVelocity = new Vector2(direction * arrowSpeed * -1f, 0f);
        }

        // 화살 스프라이트/회전 맞추기
        arrow.transform.localScale = new Vector3(direction * -1f, 1f, 1f);
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        // isAttacking이 false가 되면, 다음 Update()에서 isActiveAI가 재평가됩니다.
    }

    void SetAnim(string animName, bool loop = true)
    {
        if (spinePlayer && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
            currentAnim = animName;
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