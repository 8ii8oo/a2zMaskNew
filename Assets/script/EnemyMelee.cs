using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine; 

// EnemyMove를 상속받아 이동/순찰/낭떠러지 체크 기능을 재사용합니다.
public class EnemyMelee : EnemyMove 
{
  int attackCount = 0;
    [Header("== 근접 공격 설정 ==")]
    public float chaseRange = 6.0f;  // 플레이어 감지(추적) 범위
    public float attackRange = 0.7f; // 공격 가능 범위  
    public float attackCooldown = 1.5f; // 공격 쿨타임
    
    [Header("== 근접 피해 설정 ==")]
    public GameObject nomalDamageObj;  //데미지 판정 자식 오브젝트 (Inspector 할당)
    public GameObject skillDamageObj;
    public float damageDuration = 0.5f; //데미지 판정 유지 시간
    
    private Transform player;
    private bool isAttacking = false;
    private bool isCoolingDown = false; 

    // ⭐ 콜라이더 오프셋 관련 변수 제거

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

        if (nomalDamageObj != null)
        {
            nomalDamageObj.SetActive(false);
            // BoxCollider2D 오프셋 저장 로직 제거
         }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (player == null || isAttacking || isCoolingDown) return;
        
        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);

        if (distY > 1.0f)
        {
            isActiveAI = true;
            return;
        }

        if (distX <= attackRange)
        {
            isActiveAI = false; 
            rigid.linearVelocity = Vector2.zero; 
            LookAtPlayer(); 
            Attack(); 
        }
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
    
    // ⭐ LookAtPlayer 함수: 몬스터 방향 및 데미지 오브젝트 로컬 위치 반전
    void LookAtPlayer()
    {
        if (player == null || spinePlayer == null) return;

        float dir = player.position.x - transform.position.x;
        float s = Mathf.Sign(dir); // s는 1(오른쪽) 또는 -1(왼쪽)
        float scaleX = s * -1; // 몬스터가 바라보는 방향 스케일

        spinePlayer.skeleton.ScaleX = scaleX;
        nextMove = (int)s;
        
        // ⭐ 데미지 판정 오브젝트의 로컬 위치를 몬스터 방향에 맞게 변경
        if (nomalDamageObj != null)
        {
            // 몬스터 방향(s)에 따라 로컬 X 위치를 0.5 또는 -0.5로 설정
            float targetLocalX = s * 1.4f; 
            
            // 기존 로컬 Y, Z 위치는 유지하고 X만 변경
            Vector3 newLocalPos = nomalDamageObj.transform.localPosition;
            newLocalPos.x = targetLocalX;
            nomalDamageObj.transform.localPosition = newLocalPos;
        }
          if (skillDamageObj != null)
          {
            float targetLocalX = s * 1.6f; // 스킬 공격은 범위가 더 넓으니 값 조절 가능
            Vector3 pos = skillDamageObj.transform.localPosition;
            pos.x = targetLocalX;
            skillDamageObj.transform.localPosition = pos;
          }
    }
    
    void Attack()
    {
        if (spinePlayer == null) return;
        if (isAttacking) return;

        attackCount++;

         if(attackCount > 3)
        {
            attackCount = 0;
            isAttacking = true;
            SetAnim("skill", false); 
            StartCoroutine(ActivateSkillDamage()); 
        }
        else
        {
          isAttacking = true;
          SetAnim("attack", false);
          StartCoroutine(ActivateDamage());
        }
    }

        

    void OnAnimComplete(TrackEntry track) 
    {
        if (track.Animation.Name == "attack" || track.Animation.Name == "skill")
        {
            isAttacking = false; 
            StartCoroutine(AttackDelay());
            SetAnim("idle");     
        }
    }
    
    //데미지 판정 오브젝트 활성화/비활성화 제어
        IEnumerator ActivateDamage()
    {
         if (nomalDamageObj == null) yield break;

          yield return new WaitForSeconds(0.4f);

         nomalDamageObj.SetActive(true);

         yield return new WaitForSeconds(damageDuration);

         nomalDamageObj.SetActive(false);
         
    }

    IEnumerator ActivateSkillDamage()
{
    if (skillDamageObj == null) yield break;

    yield return new WaitForSeconds(0.3f); // 스킬 공격 발동 타이밍
    skillDamageObj.SetActive(true);

    yield return new WaitForSeconds(damageDuration + 0.5f); // 스킬은 좀 더 길게 유지 가능
    skillDamageObj.SetActive(false);
}

    IEnumerator AttackDelay()
    {
        isCoolingDown = true; 
        yield return new WaitForSeconds(attackCooldown); 
        isCoolingDown = false; 
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