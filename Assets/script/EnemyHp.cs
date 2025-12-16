using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System.Collections;

public class EnemyHp : MonoBehaviour
{
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 0.2f;
    public float EnemyMaxHp = 100f;
    public float Hp;

    public Image hpBar;
    public Image BackHpBar;
    bool isDead = false;

    private GameObject clearPanel;
    public bool isBoss = false;

    private CanvasGroup clearGroup;
    public GameObject playerHp;

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

        Hp -= damage;

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

        var move = GetComponent<EnemyMove>();
        if (move != null)
            move.isDead = true;

        if (isBoss)
        {
            StartCoroutine(BossDeathSequence());
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }

        if (playerHp != null)
        {
            Destroy(playerHp);
            playerHp.SetActive(false);
        }
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
}
