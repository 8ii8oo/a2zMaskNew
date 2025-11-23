using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Spine.Unity;

public class PortalTransition : MonoBehaviour
{
    private static PortalTransition instance;

    public Renderer filterRD;
    public SkeletonAnimation spinePlayer;
    public GameObject filter;

    private string speedProp = "_speed";
    private string scaleProp = "_scale";

    private string nextScene;
    private GameObject player;
    

    public static void BeginTransition(GameObject playerObj, string sceneName)
    {
        if (instance != null) return;

        // Resources/PortalTransition.prefab 에 있다고 가정
        GameObject obj = Instantiate(Resources.Load<GameObject>("Portal"));
        instance = obj.GetComponent<PortalTransition>();

        instance.player = playerObj;
        instance.nextScene = sceneName;

        instance.StartCoroutine(instance.Transition());
    }

    IEnumerator Transition()
    {
        DontDestroyOnLoad(gameObject);

        // 플레이어 정지
        var rigid = player.GetComponent<Rigidbody2D>();
        if (rigid) rigid.simulated = false;

        var move = player.GetComponent<PlayerMove>();
        if (move) move.enabled = false;

        // 필터 시작
        filter.SetActive(true);
        spinePlayer.AnimationState.SetAnimation(0, "idle", true);

        float duration = 3f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float s = Mathf.Lerp(0, 50, t);
            float sp = Mathf.Lerp(0, 3, t);

            filterRD.sharedMaterial.SetFloat(speedProp, sp);
            filterRD.sharedMaterial.SetFloat(scaleProp, s);

            time += Time.deltaTime;
            yield return null;
        }

        SceneManager.sceneLoaded += OnLoaded;
        SceneManager.LoadScene(nextScene);
    }

    void OnLoaded(Scene sc, LoadSceneMode mode)
    {
        StartCoroutine(Restore());
        SceneManager.sceneLoaded -= OnLoaded;
    }

    IEnumerator Restore()
    {
        float duration = 3f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float s = Mathf.Lerp(50, 0, t);
            float sp = Mathf.Lerp(3, 0, t);

            filterRD.sharedMaterial.SetFloat(speedProp, sp);
            filterRD.sharedMaterial.SetFloat(scaleProp, s);

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        instance = null;
    }
}
