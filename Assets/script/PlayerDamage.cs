using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    public float damageAmount = 10f;

    private HashSet<EnemyHp> hitEnemies = new HashSet<EnemyHp>(); 

    public bool isBlackSkill = false; 
    

    private bool hasHitTarget = false; 

    void OnEnable()
    {
        hitEnemies.Clear(); 
        isBlackSkill = false;
        hasHitTarget = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHp enemy = other.GetComponent<EnemyHp>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyHp>();
        if (enemy == null) return;

        if (isBlackSkill)
        {
            if (hasHitTarget) 
            {
                return; 
            }

            hasHitTarget = true;
            gameObject.SetActive(false); 
        }
        else // 일반 공격
        {
            if (hitEnemies.Contains(enemy)) return;
            hitEnemies.Add(enemy);
        }

        enemy.TakeDamage(damageAmount);
    }
    
    public void SetDamage(float dmg)
    {
        damageAmount = dmg;
    }
}