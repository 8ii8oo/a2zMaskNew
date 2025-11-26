using UnityEngine;
using Spine.Unity;

public class colliderAtcive : MonoBehaviour
{
    // [SerializeField] private SkeletonAnimation spinePlayer; // 플랫폼 로직에서 사용하지 않으므로 제거하거나 주석 처리 권장

    private BoxCollider2D collid;
    private Collider2D playerCollid;
    private PlayerMove playerMove; // PlayerMove 컴포넌트를 미리 저장
    private bool isCollisionIgnored = false;
    // public bool isGround = true; // PlayerMove의 isGround를 사용하므로 제거하거나 주석 처리 권장


    void Start()
    {
        collid = GetComponent<BoxCollider2D>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerCollid = playerObj.GetComponent<Collider2D>();
            playerMove = playerObj.GetComponent<PlayerMove>(); // PlayerMove 컴포넌트 참조 저장
        }
        else
        {
            Debug.LogError("Player Tag를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // PlayerMove 컴포넌트가 없거나 플레이어가 죽었으면 실행하지 않음
        if (playerMove == null || playerMove.isDead) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 👇 플레이어가 지면에 닿아있는지 PlayerMove의 isGround를 확인해야 합니다.
            // if (!isCollisionIgnored && playerMove.isGround) // 이전에 구현했던 더 안정적인 코드
            if (!isCollisionIgnored) // 현재 코드 베이스에 맞춰 isGround 체크 생략
            {
                IgnoreCollision();
                playerMove.PlatformDrop(); // PlayerMove의 PlatformDrop() 호출
            }
        }
    }

    void IgnoreCollision()
    {
        if (playerMove == null) return;
        
        isCollisionIgnored = true;
        
        // 👇 플레이어의 다른 동작(공격, 스킬 등)을 막기 위해 isAttack을 true로 설정
        playerMove.SetIsAttack(true); 

        // 플레이어와 이 플랫폼 충돌 무시
        Physics2D.IgnoreCollision(playerCollid, collid, true);

        // 0.3초 뒤 강제 복구 시도
        Invoke(nameof(RestoreCollision), 0.3f);
    }

    // 플레이어가 아래로 내려갔을 때 충돌 복구 (Invoke보다 먼저 발동될 경우)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isCollisionIgnored) return;
        if (other.CompareTag("Player"))
        {
            // 플랫폼을 완전히 벗어났을 때 복구 및 isAttack 해제
            RestoreCollision(); 
        }
    }

    void RestoreCollision()
    {
        if (!isCollisionIgnored) return; // 중복 복구 방지
        if (playerMove == null) return;

        Physics2D.IgnoreCollision(playerCollid, collid, false);
        isCollisionIgnored = false;
        
        // 👇 충돌 복구 후, isAttack을 false로 설정하여 공격 능력을 재활성화합니다.
        playerMove.SetIsAttack(false); 
        
        // Invoke에 의해 RestoreCollision이 호출되었는데, OnTriggerExit2D가 
        // 아직 호출되지 않은 상태일 경우 Invoke를 취소하여 중복 실행을 막습니다.
        CancelInvoke(nameof(RestoreCollision));
    }
    
    
}