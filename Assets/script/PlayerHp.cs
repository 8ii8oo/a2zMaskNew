using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine.Unity;
using Spine;




public class PlayerHp : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    public Skeleton skeleton;
    public GameObject startPosition;
    // 최대 HP는 인스펙터에서 설정 가능
    public float MaxHp = 100f; 
    public static float hp = 100f;

    private SpriteRenderer sr;



    public Image hpBar; 
    
    // 게임 오버 관련 UI
    public Image fadeImage;
    public GameObject returnButton;
    public GameObject gmaeOverImage;
    
    private bool isDead = false;
    private PlayerMove playerMove;
    public bool backHpHit = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitHpByScene(scene.name);

        // HP바 갱신
        if (hpBar != null)
            hpBar.fillAmount = hp / MaxHp;

        // 위치 초기화
        if (startPosition != null)
            transform.position = startPosition.transform.position;
    }

    void InitHpByScene(string sceneName)
{
    if (sceneName == "Stage21" || sceneName == "Boss")
        {
            hp += 30f;
        }
        else if (sceneName == "tutorial" || sceneName == "Stage11")
        {
            hp = MaxHp;
        }
        else if (sceneName == "Stage22" || sceneName == "Stage12")
        {
            hp += 10f;
        }

        hp = Mathf.Min(hp, MaxHp);
}



    
    void Start()
    {

        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        
        string currentScene = SceneManager.GetActiveScene().name;
        
        

        if(startPosition != null)
        {
              gameObject.transform.position = startPosition.transform.position;  
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
        StartCoroutine(DamageEffect());
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Attack);
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

        if (playerMove != null)
        {
            playerMove.KillAni();
        }

        Time.timeScale = 0.2f; 

        yield return new WaitForSecondsRealtime(2f);
        

        
        StartCoroutine(FadeToBlack());
    }
    
    IEnumerator FadeToBlack() //페이드아웃
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

    IEnumerator DamageEffect()
    {

        for(int i = 0; i < 2; i++)
        {
           
            skeletonAnimation.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(0.2f);
            skeletonAnimation.skeleton.SetColor(Color.white);
            yield return new WaitForSeconds(0.2f);
        }
    }
}