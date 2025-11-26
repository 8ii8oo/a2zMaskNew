using System.Collections;
using UnityEngine;
using Spine;
using Spine.Unity;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spinePlayer;

    [Header("이동 및 점프")]
    public float speed = 5f;
    public float jumpPower = 11f;
    private int maxJumpCount = 2;
    public int currentJumpCount = 0;
    public bool isGround = true;

    [Header("대시")]
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashingCooldown = 1f;
    bool canDash = true;
    bool dashing = false;

    [Header("가면")]
    public GameObject ImNormal;
    public GameObject ImRed;
    public GameObject ImBlue;
    public GameObject ImBlack;
    bool isNormal = true;
    bool isRed = false;
    bool isBlue = false;
    bool isBlack = false;

    bool isAttack; // 공격, 스킬, 플랫폼 드롭 시 True
    public bool isDead = false;

    [Header("스킬")]
    public SkeletonAnimation skeletonAnimation;

    [Header("컴포넌트 및 상태")]
    public Rigidbody2D rigid;
    public Collider2D groundCheckCollider; // 지면 체크용 콜라이더 (BoxCollider2D 권장)
    public LayerMask whatIsGround; // 지면 레이어 설정
    private float moveInput = 0f;
    private bool isFacingRight = true;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (spinePlayer != null)
        {
            SetAnimationState("idle");
        }

        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
        {
            skeletonAnimation.skeleton.SetSkin("normal");
            skeletonAnimation.skeleton.SetupPoseSlots();
        }

        if (ImNormal != null) ImNormal.SetActive(true);
    }

    void Update()
    {
        if (isDead) return;
        
        // 이동 입력 (공격/대시 중에는 입력 무시)
        moveInput = 0f;
        if (!isAttack && !dashing)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveInput = 1f;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveInput = -1f;
            }
        }
        
        // 지상 애니메이션 (idle/walk)
        if (isGround && !dashing && !isAttack)
        {
            if (moveInput != 0f)
            {
                SetAnimationState("walk");
            }
            else
            {
                SetAnimationState("idle");
            }
        }

        // 일반 공격 (A키)
        if (Input.GetKeyDown(KeyCode.A) && !dashing && !isAttack && isGround)
        {
            isAttack = true;
            var track = spinePlayer.AnimationState.SetAnimation(0, "attack", false);
            // 공격 애니메이션 완료 시 isAttack 해제 및 상태 복귀
            track.Complete += (trackEntry) =>
            {
                isAttack = false;
                OnActionComplete(); // 메서드 호출로 상태 복귀
            };
        }

        // 점프 (Space바)
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount && !isAttack && !dashing)
        {
            Jump();
        }

        // 대시 (LeftShift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isAttack)
        {
            StartCoroutine(Dash());
        }

        // 스킨 전환 (Q키)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 스킨/이미지 전환 로직 (이전 로직 유지)
            if (isNormal)
            {
                isRed = true; isNormal = false;
                if (ImRed != null) ImRed.SetActive(true);
                if (ImNormal != null) ImNormal.SetActive(false);
                if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
                {
                    skeletonAnimation.skeleton.SetSkin("red");
                    skeletonAnimation.skeleton.SetupPoseSlots();
                }
            }
            else if (isRed)
            {
                isBlue = true; isRed = false;
                if (ImBlue != null) ImBlue.SetActive(true);
                if (ImRed != null) ImRed.SetActive(false);
                if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
                {
                    skeletonAnimation.skeleton.SetSkin("blue");
                    skeletonAnimation.skeleton.SetupPoseSlots();
                }
            }
            else if (isBlue)
            {
                isBlack = true; isBlue = false;
                if (ImBlack != null) ImBlack.SetActive(true);
                if (ImBlue != null) ImBlue.SetActive(false);
                if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
                {
                    skeletonAnimation.skeleton.SetSkin("black");
                    skeletonAnimation.skeleton.SetupPoseSlots();
                }
            }
            else
            {
                isNormal = true; isBlack = false;
                if (ImNormal != null) ImNormal.SetActive(true);
                if (ImBlack != null) ImBlack.SetActive(false);
                if (skeletonAnimation != null && skeletonAnimation.skeleton != null)
                {
                    skeletonAnimation.skeleton.SetSkin("normal");
                    skeletonAnimation.skeleton.SetupPoseSlots();
                }
            }
        }

        if (!dashing)
        {
            Flip();
        }

        // 스킬 (S키)
        if (Input.GetKeyDown(KeyCode.S) && !dashing && !isAttack && isGround)
        {
            isAttack = true;
            Spine.TrackEntry track = null;

            if (isNormal) { track = spinePlayer.AnimationState.SetAnimation(0, "skill_normal", false); }
            else if (isRed) { track = spinePlayer.AnimationState.SetAnimation(0, "skill_red", false); }
            else if (isBlue) { track = spinePlayer.AnimationState.SetAnimation(0, "skill_blue", false); }
            else { track = spinePlayer.AnimationState.SetAnimation(0, "skill_black", false); }

            if (track != null)
            {
                track.Complete += (trackEntry) =>
                {
                    isAttack = false;
                    OnActionComplete(); // 메서드 호출로 상태 복귀
                };
            }
        }
    } // Update 종료

    void FixedUpdate()
    {
        if (!dashing)
        {
            // Rigidbody2D.velocity 사용
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
            GroundCheck();
        }
    }



    void Jump()
    {
        SetAnimationState("jump", false);
        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpPower);
        currentJumpCount++;
        isGround = false;
    }

    void Flip()
    {
        if ((isFacingRight && moveInput < 0f) || (!isFacingRight && moveInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // 착지 이벤트 핸들러
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isAttack = true;
            currentJumpCount = 0;
            isGround = true;
            // landing 애니메이션을 설정하고 완료 이벤트를 연결합니다.
            var track = spinePlayer.AnimationState.SetAnimation(0, "landing", false);
            track.Complete += OnLandingComplete; // 메서드 연결
            isAttack = false;
        }
    }
    

    private void OnLandingComplete(TrackEntry trackEntry)
    {
        // landing 애니메이션 완료 후 바로 다음 애니메이션으로 전환
        if (!dashing && !isAttack) 
        {
            if (moveInput != 0f) SetAnimationState("walk");
            else SetAnimationState("idle");
        }
    }
    
    // ⭐ 공격/스킬/대시 완료 후 호출될 메서드
    private void OnActionComplete()
    {
        // 지상에 있다면 idle/walk로 복귀
        if (isGround && !dashing)
        {
            if (moveInput != 0) SetAnimationState("walk");
            else SetAnimationState("idle");
        }
        // 공중이라면 fall 애니메이션으로 복귀
        else if (!isGround && !dashing)
        {
            if (rigid.linearVelocity.y < -0.1f)
            {
                SetAnimationState("landing", true);
            }
           
        }
    }

    void playerAttack()
    {
        // mask.SetActive(!mask.activeSelf);
    }

    IEnumerator Dash()
    {
        canDash = false;
        dashing = true;

        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0f;

        float dashDirection = isFacingRight ? 1f : -1f;
        rigid.linearVelocity = new Vector2(dashDirection * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

        rigid.gravityScale = originalGravity;

        OnActionComplete(); // 대시 완료 후 애니메이션 상태 복귀

        dashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void SetAnimationState(string animName, bool loop = true)
    {
        if (isDead && animName != "dead") return;
        if (spinePlayer == null) return;

        // 현재 트랙의 애니메이션 이름을 안전하게 가져와 비교
        string currentAnim = spinePlayer.AnimationState.GetCurrent(0)?.Animation?.Name;
        if (currentAnim == animName) return;

        spinePlayer.AnimationState.SetAnimation(0, animName, loop);
    }

    // GroundCheck() 함수 (이미지 로직 반영)
    void GroundCheck()
    {
        if (groundCheckCollider == null) return; 

        BoxCollider2D boxCollider = groundCheckCollider as BoxCollider2D;
        if (boxCollider == null)
        {
            isGround = false; 
            return;
        }

        Vector2 origin = boxCollider.bounds.center;
        Vector2 size = boxCollider.bounds.size;
        float checkDistance = 0.2f;

        RaycastHit2D GroundBoxHit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, checkDistance, whatIsGround);

        if (GroundBoxHit)
        {
            if (GroundBoxHit.collider.CompareTag("MovingPlatform") || GroundBoxHit.collider.CompareTag("OneWayPlatform"))
            {
                // 플랫폼의 상단이 플레이어의 하단보다 약간 위에 있을 때만 isGround = true
                isGround = GroundBoxHit.collider.bounds.max.y <= boxCollider.bounds.min.y + 0.1f;
            }
            else
            {
                isGround = true; 
            }
        }
        else
        {
            isGround = false;
        }
    }

    // 플랫폼 드롭 시작 시 호출되는 함수
    public void PlatformDrop()
    {
        SetAnimationState("landing", true);
        isGround = false;
    }

    public void KillAni()
    {
        isDead = true;
        SetAnimationState("dead", false);
    }
    
    // 외부에서 isAttack 상태를 설정하는 메서드 (colliderAtcive.cs에서 사용됨)
    public  void SetIsAttack(bool state)
    {
        isAttack = state;

        // 공격 상태가 해제될 때 (state == false)
        if (!isAttack)
        {
            OnActionComplete(); // 애니메이션 상태 복귀
        }
    }

}