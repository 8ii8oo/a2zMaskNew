using UnityEngine;
using Spine.Unity;

public class colliderAtcive : MonoBehaviour
{
    private Collider2D collid; 
    private Collider2D playerCollid;
    private PlayerMove playerMove;
    private bool isCollisionIgnored = false;

    void Start()
    {
        collid = GetComponent<Collider2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerCollid = playerObj.GetComponent<Collider2D>();
            playerMove = playerObj.GetComponent<PlayerMove>();
        }
    }

    void Update()
    {
        if (playerMove == null || playerMove.isDead || !playerMove.enabled) return;

        // DownGround 위에 올라가 있을 때만 하향점프 허용
        if (collid.CompareTag("DownGround") && playerMove.currentGroundTag == "DownGround")
        {
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space) && !isCollisionIgnored)
            {
                IgnoreCollision();
                playerMove.PlatformDrop();
            }
        }
    }

    void IgnoreCollision()
    {
        if (playerMove == null) return;

        Physics2D.IgnoreCollision(playerCollid, collid, true);

        isCollisionIgnored = true;

        // 0.3초 동안은 무조건 충돌 OFF 유지 → 안전하게 아래로 통과 가능
        CancelInvoke(nameof(RestoreCollision));
        Invoke(nameof(RestoreCollision), 0.3f);
    }

    void RestoreCollision()
    {
        if (!isCollisionIgnored) return;

        Physics2D.IgnoreCollision(playerCollid, collid, false);
        isCollisionIgnored = false;
    }
}
