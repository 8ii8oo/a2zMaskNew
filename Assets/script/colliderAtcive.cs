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

        if (!playerMove.enabled) return;

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isCollisionIgnored) 
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

        playerMove.SetIsAttack(true); 

        Physics2D.IgnoreCollision(playerCollid, collid, true);

        Invoke(nameof(RestoreCollision), 0.3f);
    }

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
        if (!isCollisionIgnored) return;
        if (playerMove == null) return;

        Physics2D.IgnoreCollision(playerCollid, collid, false);
        isCollisionIgnored = false;

        playerMove.SetIsAttack(false);

        CancelInvoke(nameof(RestoreCollision));
    }
}

