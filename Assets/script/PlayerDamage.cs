// PlayerDamage.cs (수정된 코드)

using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public float damageAmount = 10f; 
    private bool hasHit = false;


    void OnEnable()
    {
        hasHit = false;   // 활성화될 때마다 초기화
    }

   void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; 
        if (other.CompareTag("Enemy"))
        {
            EnemyHp enemy = other.GetComponent<EnemyHp>();
            if (enemy == null) enemy = other.GetComponentInParent<EnemyHp>();

            if (enemy != null)
            {
                hasHit = true;         
                enemy.TakeDamage(damageAmount);
                gameObject.SetActive(false); 
            }
        }
    }

    public void SetDamage(float dmg)
    {
        damageAmount = dmg;
    }
}