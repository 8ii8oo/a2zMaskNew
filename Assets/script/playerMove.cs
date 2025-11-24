using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

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
        skeletonAnimation.skeleton.SetSkin("normal");
        skeletonAnimation.skeleton.SetupPoseSlots();

        ImNormal.SetActive(true);
    }

    void Update()
    {

        
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
            if(isNormal)
            {
                isRed = true;
                isNormal = false;

                ImRed.SetActive(true);
                ImNormal.SetActive(false);

                skeletonAnimation = GetComponent<SkeletonAnimation>();
                skeletonAnimation.skeleton.SetSkin("red");    
                skeletonAnimation.skeleton.SetupPoseSlots();
            }
            else if(isRed)
            {
                isBlue = true;
                isRed = false;
                
                ImBlue.SetActive(true);
                ImRed.SetActive(false);

                skeletonAnimation = GetComponent<SkeletonAnimation>();
                skeletonAnimation.skeleton.SetSkin("blue");
                skeletonAnimation.skeleton.SetupPoseSlots();
            }
            else if(isBlue)
            {
                isBlack = true;
                isBlue = false;

                ImBlack.SetActive(true);
                ImBlue.SetActive(false);

                skeletonAnimation = GetComponent<SkeletonAnimation>();
                skeletonAnimation.skeleton.SetSkin("black");
                skeletonAnimation.skeleton.SetupPoseSlots();
            }
            else
            {
                isNormal = true;
                isBlack = false;

                ImNormal.SetActive(true);
                ImBlack.SetActive(false);

                skeletonAnimation = GetComponent<SkeletonAnimation>();
                skeletonAnimation.skeleton.SetSkin("normal");
                skeletonAnimation.skeleton.SetupPoseSlots();
            }
        }


        if (!dashing)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (!dashing)
        {

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

        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
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
                if(moveInput != 0f)
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
        if (spinePlayer != null && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
        }
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

        // BoxCast를 통해 지면 레이어와 닿았는지 여부로 isGround 설정
        if (hit.collider != null)
        {
            // 지면에 닿았고, 현재 점프 카운트가 최대치일 경우에만 초기화 (이중 점프 중 초기화 방지)
            if (currentJumpCount != 0 && isGround == false) // 공중에서 착지하는 순간
            {
                spinePlayer.AnimationState.SetAnimation(0, "landing", false);
                currentJumpCount = 0;
            }
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

}