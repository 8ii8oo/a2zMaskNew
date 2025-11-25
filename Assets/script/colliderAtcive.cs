using UnityEngine;
using Spine.Unity;


public class colliderAtcive : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spinePlayer;

    private BoxCollider2D collid;
    private Collider2D playerCollid;
    private bool isCollisionIgnored = false;
    public bool isGround = true;


    void Start()
    {
        collid = GetComponent<BoxCollider2D>();
        playerCollid = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    }

        void Update()
    {
        // 플레이어가 죽었는지 확인
        PlayerMove pm = playerCollid.GetComponent<PlayerMove>();
        if (pm != null && pm.isDead) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isCollisionIgnored)
            {
                IgnoreCollision();
                pm.PlatformDrop();
            }
        }
    }

    

    void IgnoreCollision()
    {
        isCollisionIgnored = true;

        // 플레이어와 이 플랫폼 충돌 무시
        Physics2D.IgnoreCollision(playerCollid, collid, true);

        // 0.3초 뒤 강제 복구 시도
        Invoke(nameof(RestoreCollision), 0.3f);
    }

    // 플레이어가 아래로 내려갔을 때 충돌 복구
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isCollisionIgnored) return;

        if (other.CompareTag("Player"))
        {
            RestoreCollision();
        }
    }

    void RestoreCollision()
    {
        if (!isCollisionIgnored) return; // 중복 복구 방지

        Physics2D.IgnoreCollision(playerCollid, collid, false);
        isCollisionIgnored = false;
    }
}

