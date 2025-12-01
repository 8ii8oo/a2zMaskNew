using UnityEngine;


public class EnemyDamage : MonoBehaviour
{
    public float damageAmount = 5f;
    public bool autoLookAtPlayer = false;
    private Transform player;
    public bool isProjectile = false; 

    void Start()
    {
        if (autoLookAtPlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!autoLookAtPlayer || player == null) return;

        float dir = player.position.x - transform.position.x;
        float sign = Mathf.Sign(dir);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * sign;   // 좌우 반전
        transform.localScale = scale;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHp playerHealth = other.GetComponent<PlayerHp>();
            
            if(playerHealth != null)
            {
               
                playerHealth.TakeDamage(damageAmount); 
                
                // 데미지 후 처리
                if (isProjectile)
                {
                    //화살
                    Destroy(gameObject);
                }
                else
                {
                    //근접
                    gameObject.SetActive(false);
                }
            }
        }
        
      
        else if (isProjectile && !other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    void GetDamage()
    {
        
    }
    
       public void SetDamage(float damage)
    {
        damageAmount = damage;
    }
}