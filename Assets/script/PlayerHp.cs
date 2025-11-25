using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    public float hp = 100f;
    public GameObject overUI;
    public Image fadeImage;
    public GameObject returnButton;
    public GameObject gmaeOverImage;
    private bool isDead = false;

    private PlayerMove playerMove;

    
    void Start()
    {
        //게임 오버 UI 비활성화
        if(overUI != null) overUI.SetActive(false);
        playerMove = GetComponent<PlayerMove>();
    }

    
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        hp -= damage;
        
        // HP 변화 확인용

        if(hp <= 0)
        {
            StartCoroutine(KillPlayer());
        }
    }

    IEnumerator KillPlayer()
    {
        isDead = true;

        if (playerMove != null)
        {
            playerMove.KillAni();
        }

        Time.timeScale = 0.2f; 

        yield return new WaitForSecondsRealtime(2f);
        
        if(overUI != null) overUI.SetActive(true);
        
        StartCoroutine(FadeToBlack());
    }
    
    IEnumerator FadeToBlack()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        float fadeDuration = 1.5f;
        float timer = 0f;

        // Color(R, G, B, A)는 0.0f ~ 1.0f 범위 사용
        Color startColor = new Color(1f, 1f, 1f, 0f); 
        Color endColor = new Color(1f, 1f, 1f, 1f);

        if(fadeImage != null) fadeImage.color = startColor;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 

            float progress = Mathf.Clamp01(timer / fadeDuration);
            if(fadeImage != null) fadeImage.color = Color.Lerp(startColor, endColor, progress);

            yield return null;
        }
        
        if(fadeImage != null) fadeImage.color = endColor;
        Time.timeScale = 0f;
    }
}