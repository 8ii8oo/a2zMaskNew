using System.Collections;
using UnityEngine;

public class EnemyPotal : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject wizardPrefab;

    [Header("스폰")]
    [SerializeField] private float spawnInterval = 2f; //소환 시간
    [SerializeField] private float spawnYOffset = 0.9f;

    private FadeRenderer fadeRenderer;

    private void Awake()
    {
        fadeRenderer = GetComponent<FadeRenderer>();
    }

    private void Start()
    {
        if (fadeRenderer != null)
            fadeRenderer.FadeIn();

      
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        GameObject[] spawnOrder = 
        {
            archerPrefab,
            warriorPrefab,
            wizardPrefab
        };

        foreach (GameObject prefab in spawnOrder)
        {

            yield return new WaitForSeconds(spawnInterval);
            Vector3 spawnPos = transform.position + Vector3.down * spawnYOffset;
            Instantiate(prefab, spawnPos, Quaternion.identity);

            
        }

        if (fadeRenderer != null)
            fadeRenderer.FadeOut();
    }
}
