using UnityEngine;

// 이 스크립트는 모든 적의 데미지 판정 오브젝트(근접 공격, 투사체 등)에 붙여 사용합니다.
public class EnemyDamage : MonoBehaviour
{
    public float damageAmount = 50f;
    // ⭐ 투사체라면 체크 (체크 시 충돌 후 Destroy, 미체크 시 SetActive(false))
    public bool isProjectile = false; 

    void OnTriggerEnter2D(Collider2D other)
    {
        // ⭐ 충돌한 상대가 플레이어인지 확인
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHp playerHealth = other.GetComponent<PlayerHp>();
            
            if(playerHealth != null)
            {
                // PlayerHp의 TakeDamage 함수를 호출하여 데미지 적용
                playerHealth.TakeDamage(damageAmount); 
                
                // ⭐ 데미지 적용 후 처리
                if (isProjectile)
                {
                    // 투사체(화살)라면 오브젝트를 파괴합니다.
                    Destroy(gameObject);
                }
                else
                {
                    // 근접 판정이라면 비활성화합니다.
                    gameObject.SetActive(false);
                }
            }
        }
        
        // ⭐ 투사체인 경우, 플레이어가 아닌 다른 물체(벽, 땅)에 닿아도 파괴
        // 'Enemy' 태그가 붙지 않은 모든 물체에 닿았을 때 파괴 (근접 공격 오브젝트에는 적용 안 됨)
        else if (isProjectile && !other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
    
    // ⭐ 외부(EnemyArcher)에서 데미지 값을 설정할 수 있도록 public 함수 추가
    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }
}