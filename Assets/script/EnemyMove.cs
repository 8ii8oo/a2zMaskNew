using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation spinePlayer;
    protected Rigidbody2D rigid;

    [Header("== 이동 설정 ==")]
    public float speed = 1f;
    public int nextMove = 1;         // 1: 오른쪽, -1: 왼쪽
    protected bool isStopping = false;
    protected string currentAnim = "";

    [HideInInspector] public bool isActiveAI = true;
    

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (spinePlayer != null) 
            SetAnim("idle");
            nextMove = 1;

    }

    protected virtual void FixedUpdate()
    {
        if (!isActiveAI) return;     // 자식 AI(공격 등) 활성화 시 이동 중단

        if (!isStopping)
        {
            rigid.linearVelocity = new Vector2(nextMove * speed, rigid.linearVelocity.y);
            if (nextMove != 0)
                SetAnim("walk");
        }
        else
        {
            rigid.linearVelocity = Vector2.zero;
            SetAnim("idle");
        }

        //스파인 좌우 반전
        if (nextMove != 0 && spinePlayer != null)
        {
            spinePlayer.skeleton.ScaleX = nextMove * -1;
        }

        // 낭떠러지 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Ground"));

        // 벽체크
        RaycastHit2D wallHit = Physics2D.Raycast(rigid.position, new Vector2(nextMove, 0), 1f, LayerMask.GetMask("Wall"));
        


        if (!isStopping && (rayHit.collider == null || wallHit.collider != null))
        {
            StartCoroutine(StopAndTurn());
        }

        
    }

    // 낭떠러지에서 멈춘 후 방향 반전
    protected IEnumerator StopAndTurn()
    {
        isStopping = true;
        SetAnim("idle");
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        nextMove *= -1;
        isStopping = false;
    }

    // 애니메이션 헬퍼
    protected void SetAnim(string animName, bool loop = true)
    {
        if (!spinePlayer) return;

        if (spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
            currentAnim = animName;
        }
    }
}
