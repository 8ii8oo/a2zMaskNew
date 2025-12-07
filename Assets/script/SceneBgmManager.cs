using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBgmManager : MonoBehaviour
{
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
        switch (scene.name)
        {
            case "title":
                AudioManager.instance.PlayBgm(AudioManager.Bgm.Title);
                break;
            case "Stage11":
                AudioManager.instance.PlayBgm(AudioManager.Bgm.YRoom);
                break;
            case "Stage21":
                AudioManager.instance.PlayBgm(AudioManager.Bgm.RRoom);
                break;
            case "Boss":
                AudioManager.instance.PlayBgm(AudioManager.Bgm.Boss);
                break;
        }
    }
}
