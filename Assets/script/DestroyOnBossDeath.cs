using UnityEngine;

public class DestroyOnBossDeath : MonoBehaviour
{
    private EnemyHp bossHp;

    void Start()
    {
       
        EnemyHp[] enemies = FindObjectsOfType<EnemyHp>();

        foreach (var e in enemies)
        {
            if (e.isBoss)
            {
                bossHp = e;
                break;
            }
        }
    }

    void Update()
    {
        if (bossHp == null) return;


        if (bossHp.isDead)
        {
            Destroy(gameObject);
        }
    }
}
