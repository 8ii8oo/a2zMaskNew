using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager instance;

    public GameObject QCool;
    public GameObject SCool;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSkillCooldownUI(bool active)
    {
        SCool?.SetActive(active);
    }

    public void SetSkinCooldownUI(bool active)
    {
        QCool?.SetActive(active);
    }
}
