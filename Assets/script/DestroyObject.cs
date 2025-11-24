using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가

public class DestroyObject : MonoBehaviour
{
    // ⭐ 파괴되는 시간 (예: 몬스터 공격 판정이 1초 동안 유지)
    public float destroyTime = 0.5f; 

    void Start()
    {
        // ⭐ 시작 시 바로 파괴 코루틴 호출 (근접 공격 판정 유지 시간)
        StartCoroutine(TimeDestroy(destroyTime));
    }

    // ⭐ 일정 시간 후 오브젝트를 파괴하는 코루틴
    IEnumerator TimeDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 플레이어와 충돌 시 오브젝트 파괴
    void OnTriggerEnter2D(Collider2D other)
    {
        // 몬스터의 공격 판정 오브젝트이므로, 플레이어에게 피해를 준 후 파괴합니다.
        if (other.gameObject.CompareTag("Player"))
        {
            // 여기서 Damage 로직은 EnemyDamage 스크립트에게 맡깁니다.
            Destroy(gameObject);
        }
    }
}