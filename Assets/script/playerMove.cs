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

    bool isAttack;
    public bool isDead = false;

    [Header("스킬")]
    public SkeletonAnimation skeletonAnimation;

    [Header("컴포넌트 및 상태")]
    public Rigidbody2D rigid;
    public Collider2D groundCheckCollider; // BoxCollider2D 또는 CapsuleCollider2D
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
        moveInput = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }

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

        if (Input.GetKeyDown(KeyCode.A) && !dashing && !isAttack && isGround)
        {
            isAttack = true;
            var track = spinePlayer.AnimationState.SetAnimation(0, "attack", false);
            track.Complete += (trackEntry) =>
            {
                isAttack = false;

                if (moveInput != 0)
                    SetAnimationState("walk");
                else
                    SetAnimationState("idle");
            };
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 스킨/이미지 전환 로직
            if (isNormal)
            {
                isRed = true;
                isNormal = false;

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
                isBlue = true;
                isRed = false;

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
                isBlack = true;
                isBlue = false;

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
                isNormal = true;
                isBlack = false;

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

        if (Input.GetKeyDown(KeyCode.S) && !dashing && !isAttack && isGround)
        {
            isAttack = true;
            Spine.TrackEntry track = null;

            if (isNormal)
            {
                track = spinePlayer.AnimationState.SetAnimation(0, "skill_normal", false);
            }
            else if (isRed)
            {
                track = spinePlayer.AnimationState.SetAnimation(0, "skill_red", false);
            }
            else if (isBlue)
            {
                track = spinePlayer.AnimationState.SetAnimation(0, "skill_blue", false);
            }
            else
            {
                track = spinePlayer.AnimationState.SetAnimation(0, "skill_black", false);
            }

            if (track != null)
            {
                track.Complete += (trackEntry) =>
                {
                    isAttack = false;

                    if (moveInput != 0)
                        SetAnimationState("walk");
                    else
                        SetAnimationState("idle");
                };
            }
        }
    } // <-- Update 종료

    void FixedUpdate()
    {
        if (!dashing)
        {
            // Rigidbody2D.velocity 사용
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
            GroundDheck();
        }
    }

    void Jump()
    {
        SetAnimationState("jump");

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor")) // CompareTag 사용 권장
        {
            spinePlayer.AnimationState.SetAnimation(0, "landing", false);

            currentJumpCount = 0;
            isGround = true;

            if (!dashing)
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

        if (moveInput == 0f)
        {
            rigid.linearVelocity = new Vector2(0f, rigid.linearVelocity.y);
            SetAnimationState("idle");
        }
        else
        {
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
            SetAnimationState("walk");
        }

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

    void GroundDheck()
    {
        // 콜라이더의 하단 중앙에서 아래로 작은 박스를 쏴서 지면을 감지
        if (groundCheckCollider == null) return; // 널 체크

        Vector2 origin = groundCheckCollider.bounds.center;
        Vector2 size = groundCheckCollider.bounds.size;

        // 지면을 감지하는 작은 박스 크기 및 거리 설정
        float checkDistance = 0.1f; // 아래로 체크할 거리

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, checkDistance, whatIsGround);

        // BoxCast를 통해 지면 레이어와
    }

    public void PlatformDrop()
{
    
    SetAnimationState("landing", false);

    isGround = false;
}

    public void KillAni()
    {
        
        isDead = true;
        SetAnimationState("dead");
    }

}