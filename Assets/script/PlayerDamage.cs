using UnityEngine;
using System.Collections.Generic;

public class PlayerDamage : MonoBehaviour
{
    public float damageAmount = 10f;

    // 이번 공격 동안 이미 맞은 적들 저장
    private HashSet<EnemyHp> hitEnemies = new HashSet<EnemyHp>();

    void OnEnable()
    {
        hitEnemies.Clear();   // 공격이 새로 시작되면 리셋
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHp enemy = other.GetComponent<EnemyHp>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyHp>();
        if (enemy == null) return;

        // 같은 적은 한 번만 데미지
        if (hitEnemies.Contains(enemy)) return;

        // 처음 맞는 적만 저장
        hitEnemies.Add(enemy);
        enemy.TakeDamage(damageAmount);
    }

    public void SetDamage(float dmg)
    {
        damageAmount = dmg;
    }
}
