// EnemyHp.cs
using UnityEngine;
using UnityEngine.UI; // UnityEngine.UI 추가

public class EnemyHp : MonoBehaviour
{

    private bool isInvulnerable = false; 
    public float invulnerabilityTime = 0.2f;
    public float EnemyMaxHp = 100f; // 기본값 설정
    private float Hp; // private 인스턴스 변수로 변경

    public Image hpBar;
    public Image BackHpBar;
    bool isDead = false;
    
    // Start에서 Invoke("BackHpFun", 0.5f); 관련 코드는 제거했습니다.
    // 해당 로직은 플레이어 HP의 데미지 후처리용으로 보입니다.

    void Start()
    {
        
        hpBar.enabled = false;
        BackHpBar.enabled = false;
        Hp = EnemyMaxHp;
        // hpBar가 있다면 초기 Fill Amount 설정
        if (hpBar != null)
        {
            hpBar.fillAmount = 1f;
        }

        

        
    }

    void Update()
    {
        if (hpBar != null)
        {
            // HP 바를 현재 HP 비율로 업데이트합니다.
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, Hp / EnemyMaxHp, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float damage)
    {
        hpBar.enabled = true;
        BackHpBar.enabled = true;
        if (isDead) return;

        Hp -= damage;

        Debug.Log(Hp);
        
        if(Hp <= 0) 
        {
            Hp = 0; 
            isDead = true; 
            Die();
        }
    }

    

    private void Die()
    {

        Destroy(gameObject, 0.5f); 
    }
}