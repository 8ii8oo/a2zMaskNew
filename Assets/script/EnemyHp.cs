using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System.Collections;

public class EnemyHp : MonoBehaviour
{
    [SerializeField] private GameObject enemySpawn; 
    [SerializeField] private float spawnYOffset = 0.9f;
    [SerializeField] private Transform spawnedEnemyParent;

    private bool isInvulnerable = false;
    public float invulnerabilityTime = 0.2f;
    public float EnemyMaxHp = 100f;
    public float Hp;

    public Image hpBar;
    public Image BackHpBar;
    public bool isDead = false;

    private GameObject clearPanel;
    public bool isBoss = false;

    private CanvasGroup clearGroup;
    public GameObject playerHp;
    private bool hasEnemySpawnHp = false;

    void Start()
    {
    
        if (GameManager.instance != null && GameManager.instance.gameClearPanel != null)
        {
            clearPanel = GameManager.instance.gameClearPanel;
            clearGroup = clearPanel.GetComponent<CanvasGroup>();

            if (isBoss)
            {
                if (clearGroup != null)
                {
                    clearGroup.alpha = 0f;
                    clearGroup.interactable = false;
                    clearGroup.blocksRaycasts = false;
                }

                clearPanel.SetActive(false);
            }
        }
        

        if (hpBar != null) hpBar.enabled = false;
        if (BackHpBar != null) BackHpBar.enabled = false;
        Hp = EnemyMaxHp;

        if (hpBar != null)
            hpBar.fillAmount = 1f;
    }

    void Update()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = Mathf.Lerp(
                hpBar.fillAmount,
                Hp / EnemyMaxHp,
                Time.deltaTime * 5f
            );
        }

        
    }

    public void TakeDamage(float damage)
    {
        if (hpBar != null) hpBar.enabled = true;
        if (BackHpBar == null) return;
        else BackHpBar.enabled = true;

        if (isDead) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.EnemyHit);

        float preHp = Hp;
        Hp -= damage;


        if(isBoss && !hasEnemySpawnHp && preHp > EnemyMaxHp * 0.5f && Hp <= EnemyMaxHp * 0.5f)
        {
            hasEnemySpawnHp = true;
            SpawnEnemy();
        }

        if (Hp <= 0)
        {
            Hp = 0;
            isDead = true;
            Die();
        }
    }

    private void Die()
{
    if (BackHpBar != null)
        Destroy(BackHpBar);

    if (isBoss && spawnedEnemyParent != null)
    {
        Destroy(spawnedEnemyParent.gameObject);
    }

    var move = GetComponent<EnemyMove>();
    if (move != null)
        move.isDead = true;

    if (isBoss)
        StartCoroutine(BossDeathSequence());
    else
        Destroy(gameObject, 0.5f);
}


    IEnumerator BossDeathSequence()
    {
        isDead = true;
        AudioManager.instance.PlayBgm(AudioManager.Bgm.GameClear);

        var enemyMelee = GetComponent<EnemyMelee>();
        if (enemyMelee != null)
            enemyMelee.PlayDeathAnimation();

        yield return new WaitForSeconds(4f);

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);

            if (clearGroup != null)
            {
                StartCoroutine(FadeInCanvasGroup(clearGroup));
            }
        }
    }

    IEnumerator FadeInCanvasGroup(CanvasGroup group, float duration = 2f)
    {
        float time = 0f;
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        while (group.alpha < 1f)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Clamp01(time / duration);
            yield return null;
        }

        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void SpawnEnemy()
{
    if (enemySpawn == null) return;

    Vector3 enemySpawnPos = transform.position + Vector3.up * spawnYOffset;
    GameObject enemy = Instantiate(enemySpawn, enemySpawnPos, Quaternion.identity);

    if (spawnedEnemyParent != null)
        enemy.transform.SetParent(spawnedEnemyParent);
}


    
}
