using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHp : MonoBehaviour
{
    // 최대 HP는 인스펙터에서 설정 가능
    public float MaxHp = 100f; 
    public static float hp = 100f;



    // 인스펙터에서 Fill Amount 타입의 Image 컴포넌트를 연결해야 합니다.
    public Image hpBar; 
    
    // 게임 오버 관련 UI 요소들
    public Image fadeImage;
    public GameObject returnButton;
    public GameObject gmaeOverImage;
    
    private bool isDead = false;
    private PlayerMove playerMove;
    public bool backHpHit = false;


    
    void Start()
    {

            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Stage21" || currentScene == "Boss" || currentScene == "Stage11")
        {
            hp = MaxHp;
        }

        
         if (hpBar != null)
    {
        hpBar.fillAmount = hp / MaxHp;
    }
    
        
        playerMove = GetComponent<PlayerMove>();

        
    }

    void Update()
    {

        if (hpBar != null)
        {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp / MaxHp, Time.deltaTime * 5f);
        }
        
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;

        hp -= damage;
        Invoke("BackHpFun", 0.5f);
        

        if(hp <= 0) 
        {
            hp = 0; 
            StartCoroutine(KillPlayer());
        }
    }

    void BackHpFun()
    {
        backHpHit = true;
    }

    IEnumerator KillPlayer()
    {
        isDead = true;

        // 플레이어 움직임 스크립트의 사망 애니메이션 호출
        if (playerMove != null)
        {
            playerMove.KillAni();
        }

        // 시간 느리게
        Time.timeScale = 0.2f; 

        // 플레이어 사망 애니메이션 재생 시간 대기
        yield return new WaitForSecondsRealtime(2f);
        
        // 게임 오버 UI 활성화

        
        // 페이드 아웃 코루틴 시작
        StartCoroutine(FadeToBlack());
    }
    
    IEnumerator FadeToBlack()
    {

        yield return new WaitForSecondsRealtime(1.5f); 

        float fadeDuration = 1.5f;
        float timer = 0f;

        Color startColor = new Color(255f, 255f, 255f, 0f); 
        Color endColor = new Color(255f, 255f, 255f, 1f); 
        

        if(fadeImage != null) 
        {
            fadeImage.color = startColor;
            fadeImage.gameObject.SetActive(true); 
        }

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