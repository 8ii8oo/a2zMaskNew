using System.Collections;
using UnityEngine;
using Spine;
using Spine.Unity;
using NUnit.Framework;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class PlayerMove : MonoBehaviour
{
    //제발좀
    //진짜제발
    [SerializeField] private SkeletonAnimation spinePlayer;

    public bool isPortal = false; //포탈
    public string currentGroundTag = "";
    public GameObject slashPrefab; // 인스펙터에서 참격 프리팹 연결
    public Transform firePoint; // 발사 위치 (플레이어 앞에 빈 오브젝트)

    private bool attackSoundPlayed = false; //사운드 소리 한번만 들리게

    public static PlayerMove instance;


    

    public float damageDuration = 0.5f; //데미지 판정 유지시간
    public GameObject damageObject;
    private bool isAttacking = false;
    private bool isCoolingDown = false; 
    private bool skinCooling = false; //q스킬
    public float skinCooldownTime = 8f;
    private bool skillCooling = false;
    public float skillCooldownTime = 10f; //s스킬

    public float attackCooldown = 1.5f; //공격 쿨타임



    [Header("이동 및 점프")]
    public float speed = 5f;
    public float jumpPower = 11f;
    private int maxJumpCount = 2;
    public int currentJumpCount = 0;
    public bool isGround = true;

    [Header("대시")]
    public float dashPower = 15f;
    public float dashTime = 0.2f;
    public float dashingCooldown = 1f;
    bool canDash = true;
    bool dashing = false;

    [Header("가면")]
    public GameObject ImNormal;
    public GameObject ImRed;
    public GameObject ImBlue;
    public GameObject ImBlack;

    public GameObject QCool;
    public GameObject SCool;

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
    public Collider2D groundCheckCollider;
    public LayerMask whatIsGround; // 지면 레이어 설정
    private float moveInput = 0f;
    private bool isFacingRight = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동에도 유지
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{

    GameObject spawnPointObject = GameObject.FindGameObjectWithTag("SpawnPoint");

    if (spawnPointObject != null)
    {
        transform.position = spawnPointObject.transform.position;
    }
    
    

    isPortal = false;
}

    void Start()
    {

        
        SCool.SetActive(false);
        QCool.SetActive(false);

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
        if (isPortal)
{
    moveInput = 0f;
    SetAnimationState("idle");
    return;
}
        if (isDead) return;
        
        // 이동 입력 (공격,대시 중에는 입력 무시)
        moveInput = 0f;
        if ( !dashing)
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

       if (Input.GetKeyDown(KeyCode.A) && !dashing && !isAttack && isGround)
{
    isAttack = true;
    attackSoundPlayed = false; // 새 공격 시작
    AudioManager.instance.PlaySfx(AudioManager.Sfx.Normal);

    var track = spinePlayer.AnimationState.SetAnimation(0, "attack", false);

    // 사운드 한 번만 재생
    if (!attackSoundPlayed)
    {
        
        attackSoundPlayed = true;
    }

    StartCoroutine(NormalAttackRoutine());

    track.Complete += (t) =>
    {
        isAttack = false;
        OnActionComplete();
    };
}

if (Input.GetKeyDown(KeyCode.S) && !dashing && !isAttack && isGround && !skillCooling)
{
    SCool.SetActive(true);
    StartCoroutine(SkillCooldown());
    
    isAttack = true;
    Spine.TrackEntry track = null;
    

    if (isNormal) track = spinePlayer.AnimationState.SetAnimation(0, "skill_normal", false);
    else if (isRed) track = spinePlayer.AnimationState.SetAnimation(0, "skill_red", false);
    else if (isBlue) track = spinePlayer.AnimationState.SetAnimation(0, "skill_blue", false);
    else track = spinePlayer.AnimationState.SetAnimation(0, "skill_black", false);

    StartCoroutine(SkillAttackRoutine());

    track.Complete += (t) =>
    {
        isAttack = false;
        OnActionComplete();
    };
}

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount && !isAttack && !dashing)
        {

            Jump();
        }

        // 대쉬
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isAttack)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
            StartCoroutine(Dash());
        }

        // 스킨 전환 Q키
        if (Input.GetKeyDown(KeyCode.Q) && !dashing && !isAttack && !skinCooling)
        {
            QCool.SetActive(true);
            StartCoroutine(SkinCooldown());
            ChangeSkinCycle();
            

           
        }

        if (!dashing)
        {
            Flip();
        }

        
    } // Update 종료
    IEnumerator SkillCooldown()
    {
        skillCooling = true;
        yield return new WaitForSeconds(skillCooldownTime);
        skillCooling = false;
        SCool.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!dashing)
        {
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
            GroundCheck();
        }
    }



    void Jump()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
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
        IEnumerator SkinCooldown() //q스킬쿨타임
    {
      skinCooling = true;
      AudioManager.instance.PlaySfx(AudioManager.Sfx.Mask);
       yield return new WaitForSeconds(skinCooldownTime);
      skinCooling = false;
      QCool.SetActive(false);
    }

    void ChangeSkinCycle() //가면변환
{
    if (isNormal)
    {
        isRed = true; isNormal = false;
        ImRed?.SetActive(true);
        ImNormal?.SetActive(false);
        skeletonAnimation.skeleton.SetSkin("red");
    }
    else if (isRed)
    {
        isBlue = true; isRed = false;
        ImBlue?.SetActive(true);
        ImRed?.SetActive(false);
        skeletonAnimation.skeleton.SetSkin("blue");
    }
    else if (isBlue)
    {
        isBlack = true; isBlue = false;
        ImBlack?.SetActive(true);
        ImBlue?.SetActive(false);
        skeletonAnimation.skeleton.SetSkin("black");
    }
    else
    {
        isNormal = true; isBlack = false;
        ImNormal?.SetActive(true);
        ImBlack?.SetActive(false);
        skeletonAnimation.skeleton.SetSkin("normal");
    }

    skeletonAnimation.skeleton.SetupPoseSlots();
} 

    // 착지 이벤트 핸들러
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("DownGround"))
    {
        currentGroundTag = "DownGround";
    }
    else if (collision.collider.CompareTag("floor"))
    {
        currentGroundTag = "floor";
    }
    else
    {
        currentGroundTag = collision.collider.tag;
    }

        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("DownGround") )
        {
            isAttack = true;
            currentJumpCount = 0;
            isGround = true;
        
            var track = spinePlayer.AnimationState.SetAnimation(0, "landing", false);
            track.Complete += OnLandingComplete; // 메서드 연결
            isAttack = false;
        }

        
    }

    void OnCollisionExit2D(Collision2D collision)
{
    if (!string.IsNullOrEmpty(currentGroundTag))
    {
        if (collision.collider.CompareTag(currentGroundTag))
        currentGroundTag = "";
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
    
    // 공격/스킬/대시 완료 후 호출될 메서드
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

        string currentAnim = spinePlayer.AnimationState.GetCurrent(0)?.Animation?.Name;
        if (currentAnim == animName) return;

        spinePlayer.AnimationState.SetAnimation(0, animName, loop);
    }

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

    public void PlatformDrop() //하향점프
    {
        SetAnimationState("landing", true);
        isGround = false;
    }

    public void KillAni()
    {
        isDead = true;
        SetAnimationState("dead", false);
    }
    
    public  void SetIsAttack(bool state) 
    {
        isAttack = state;

        // 공격 상태가 해제될 때
        if (!isAttack)
        {
            OnActionComplete(); // 애니메이션 상태 복귀
        }
    }

    IEnumerator NormalAttackRoutine()
{
    if (damageObject == null) yield break;

    

    float baseDamage = 10f;
    float finalDamage = baseDamage;

   

    // 타격 타이밍 (애니메이션 0.2초 후)
    yield return new WaitForSeconds(0.2f);

    damageObject.GetComponent<PlayerDamage>().SetDamage(finalDamage);
    damageObject.SetActive(true);

    yield return new WaitForSeconds(damageDuration);
    damageObject.SetActive(false);

}

IEnumerator SkillAttackRoutine() //스킬
{
    
    if (damageObject == null) yield break;

    float baseDamage = 20f; //기본 스킬 데미지 (블루랑 노멀 기본 20 데미지)
    float finalDamage = baseDamage;

    if(isNormal)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Normal);
            StartCoroutine(Dash());
        }

        if (isRed)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Red);
    }

    if (isBlue)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Blue);
        FireSlash();
    }

    if (isBlack)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Black);
    }


    if (isRed) finalDamage += 10f; //화염 +10 데미지
    if (isBlack) finalDamage += 20f; //블랙 +20 데미지 
    
     
    yield return new WaitForSeconds(0.15f); 

    damageObject.GetComponent<PlayerDamage>().SetDamage(finalDamage);
    damageObject.SetActive(true);
    if (isBlack)
    {  
        PlayerDamage pd = damageObject.GetComponent<PlayerDamage>();
        if (pd != null)
            pd.isBlackSkill = true;
    }

    yield return new WaitForSeconds(damageDuration);
    damageObject.SetActive(false);
}



void FireSlash()
{
    if (slashPrefab == null || firePoint == null) return;

    Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
    GameObject slash = Instantiate(slashPrefab, firePoint.position, Quaternion.identity);
    slash.SetActive(true);

    SpriteRenderer sr = slash.GetComponent<SpriteRenderer>();
    sr.flipX = isFacingRight;

    
    
    SlashProjectile proj = slash.GetComponent<SlashProjectile>();
    if (proj != null)
    {
        proj.Init(dir, 20f); // 데미지 설정 가능
    }
}





    

    
    IEnumerator AttackDelay()
    {
        isCoolingDown = true; 
        yield return new WaitForSeconds(attackCooldown); 
        isCoolingDown = false; 
    }

public void SetDamageObjectActive(bool state, float damage)
{
    if (damageObject == null) return;

    PlayerDamage playerDamage = damageObject.GetComponent<PlayerDamage>();
    if (playerDamage != null)
    {
        playerDamage.SetDamage(damage);
    }
    
    damageObject.SetActive(state);
}





}
