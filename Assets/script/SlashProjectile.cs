using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 2f;
    public float damage = 10f;

    private Vector2 direction = Vector2.right;

    public void Init(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // 적 태그에 맞게 조정
        {
            

            PlayerDamage enemyDamage = collision.GetComponent<PlayerDamage>();
            if (enemyDamage != null)
            {
                enemyDamage.SetDamage(damage);
            }
        }
        
    }
}
