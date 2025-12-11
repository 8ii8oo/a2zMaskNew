using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public string sceneName;
    public Image Panel;
    public float fadeDuration = 1f;
    public GameObject Illu;

    private bool isTransitioning = false; // 중복방지 
    private bool hasLoaded = false;
    public bool isFade;

    void Awake()
    {
        
    }

    void Start()
    {
        
        Time.timeScale = 1f;
        DontDestroyOnLoad(Panel.transform.root.gameObject);
    }

    public IEnumerator FadeOutAndLoad()
    {
        isFade = true;
       
        AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
        isTransitioning = true;
        Panel.gameObject.SetActive(true);

        Color alpha = Panel.color;
        float time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeDuration;
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.completed += (_) =>
        {
            StartCoroutine(FadeIn());
            hasLoaded = true;

            
        };
        yield return new WaitForSeconds(1f);
            isFade = false;

        yield return null;
    }

    IEnumerator FadeIn()
    {
        
        Color alpha = Panel.color;
        float time = 0f;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / fadeDuration;
            alpha.a = Mathf.Lerp(1, 0, time);
            Panel.color = alpha;
            Destroy(Illu);
            yield return null;
        }

        Panel.gameObject.SetActive(false);
        
    }

    void Update()
    {
        if (!isTransitioning && !hasLoaded && Input.anyKeyDown )
        {
            StartCoroutine(FadeOutAndLoad());
        }
    }

}

