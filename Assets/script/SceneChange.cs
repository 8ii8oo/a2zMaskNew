using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Image Panel;
    public float fadeDuration = 1f;
    public GameObject Illu;

    private bool isTransitioning = false;

    void Start()
    {
        Time.timeScale = 1f;

        DontDestroyOnLoad(Panel.transform.root.gameObject);

        Panel.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        isTransitioning = true;

        Panel.gameObject.SetActive(true);
        Color alpha = Panel.color;
        float time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.unscaledDeltaTime / fadeDuration;
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

  
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

      
        async.completed += (_) =>
        {
            StartCoroutine(FadeIn());
        };
    }

    IEnumerator FadeIn()
    {
        Color alpha = Panel.color;
        float time = 0f;

        if (Illu != null)
            Destroy(Illu);
      
        while (alpha.a > 0f)
        {
            time += Time.unscaledDeltaTime / fadeDuration;
            alpha.a = Mathf.Lerp(1, 0, time);
            Panel.color = alpha;
            yield return null;
        }

        Panel.gameObject.SetActive(false);
    }
}
