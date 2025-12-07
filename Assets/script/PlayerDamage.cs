using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    public float damageAmount = 10f;

    // 이번 공격 동안 이미 맞은 적들 저장
    private HashSet<EnemyHp> hitEnemies = new HashSet<EnemyHp>();

    public bool isBlackSkill = false;

    void OnEnable()
    {
        hitEnemies.Clear();   // 공격이 새로 시작되면 리셋
        isBlackSkill = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHp enemy = other.GetComponent<EnemyHp>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyHp>();
        if (enemy == null) return;

        if (hitEnemies.Contains(enemy)) return;

        hitEnemies.Add(enemy);
        enemy.TakeDamage(damageAmount);

        if(isBlackSkill)
        {
           gameObject.SetActive(false);
        }
    }

    

    public void SetDamage(float dmg)
    {
        damageAmount = dmg;
    }
}
