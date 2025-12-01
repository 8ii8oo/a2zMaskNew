using UnityEngine;
using Spine.Unity;

public class colliderAtcive : MonoBehaviour
{
    

    private EdgeCollider2D collid;
    private Collider2D playerCollid;
    private PlayerMove playerMove;
    private bool isCollisionIgnored = false;
   


    void Start()
    {
        collid = GetComponent<EdgeCollider2D>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerCollid = playerObj.GetComponent<Collider2D>();
            playerMove = playerObj.GetComponent<PlayerMove>(); 
        }

    }

    void Update()
    {

        if (playerMove == null || playerMove.isDead) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
        
            if (!isCollisionIgnored) 
            {
                IgnoreCollision();
                playerMove.PlatformDrop(); 
            }
        }
    }

    void IgnoreCollision()
    {
        if (playerMove == null) return;
        
        isCollisionIgnored = true;
        
        //플레이어의 다른 동작(공격, 스킬 등)을 막기
        playerMove.SetIsAttack(true); 

        // 플레이어와 이 플랫폼 충돌 무시
        Physics2D.IgnoreCollision(playerCollid, collid, true);

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
        
        //충돌 후 공격 능력을 재활성화
        playerMove.SetIsAttack(false); 
        
        CancelInvoke(nameof(RestoreCollision));
    }
    
    
}