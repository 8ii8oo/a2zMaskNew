// EnemyHp.cs
using UnityEngine;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{

    private bool isInvulnerable = false; 
    public float invulnerabilityTime = 0.2f;
    public float EnemyMaxHp = 100f; 
    private float Hp; 

    public Image hpBar;
    public Image BackHpBar;
    bool isDead = false;
    

    void Start()
    {
        
        hpBar.enabled = false;
        BackHpBar.enabled = false;
        Hp = EnemyMaxHp;

        if (hpBar != null)
        {
            hpBar.fillAmount = 1f;
        }

        

        
    }

    void Update()
    {
        if (hpBar != null)
        {
            // HP 바를 현재 HP 비율로 업데이트
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, Hp / EnemyMaxHp, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float damage)
    {
        hpBar.enabled = true;
        BackHpBar.enabled = true;
        if (isDead) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.EnemyHit);
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