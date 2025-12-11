
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject OverImage;
    public static GameManager instance;
    public SceneChange sceneChange;
     ScenePotal scenePotal;
    public GameObject EscSet; //esc눌렀을때 뜨는 팝업창
    public GameObject Ctrl;
    public GameObject Option;
    public static bool GameIsPaused = false; //상태.  일시정지, 창 켜져잇는지 아닌지 확인
    static bool CTSA = false; //SA = SetActive, 컨트롤
    static bool OPSA = false; //옵션
    bool isTutorial;
    public static string lastSceneName = "";
    



    void Start()
    {
    

        

        if(instance == null)
        {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
{
    
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        // 씬 전환 중 체크
        bool isSceneChanging = sceneChange != null && sceneChange.isFade;

        // 포탈 전환 중 체크
        bool isPortalFading = false;
        GameObject portalObj = GameObject.FindWithTag("Potal"); // 정확히 'Portal'
        if (portalObj != null)
        {
            ScenePotal potal = portalObj.GetComponent<ScenePotal>();
            if (potal != null && potal.isFade)
            {
                isPortalFading = true;
            }
        }

        // 전환 중이면 ESC 무시
        if (isSceneChanging || isPortalFading)
            return;

        // ESC 작동
        if (GameIsPaused)
            Resume();
        else
            pasue();
    }

    
}


    void pasue()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
        EscSet.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

   void Resume()
{
    AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
   
    if (CTSA)
    {
        Ctrl.SetActive(false);
        CTSA = false;
    }

    if (OPSA)
    {
        Option.SetActive(false);
        OPSA = false;
    }
   
    EscSet.SetActive(false);

    Time.timeScale = 1f;
    GameIsPaused = false;
}


    public void OnClickCtrl()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
        Ctrl.SetActive(!Ctrl.activeSelf);

        CTSA = Ctrl.activeSelf;
    }

    public void OnClickOption()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
        Option.SetActive(!Option.activeSelf);

        OPSA = Option.activeSelf;
    }




    public void OnClickReturn()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
        if (CTSA == true)
        {
            Ctrl.SetActive(false);
            CTSA = false;
        }

        else if (OPSA == true)
        {
            Option.SetActive(false);
            OPSA = false;
        }

        else
        {
            Resume();
        }
    }

    public void OnClickRePlay(string sceneName)
{
    AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
    Time.timeScale = 1f;

    ClearGameOverUI();

    string nextScene = "Stage11"; // 기본값

    if (lastSceneName == "tutorial")
    {
        nextScene = "tutorial"; // 튜토리얼에서 죽었으면 튜토리얼로
    }

    SceneManager.LoadScene(nextScene);
}


    public void OnClickExit()
    {
        Application.Quit();


    }

    void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
   
    if (scene.name != "Title")
    {
        if (gameObject != null && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    scenePotal = FindObjectOfType<ScenePotal>();

}

private void ClearGameOverUI()
{
    PlayerHp hp = FindObjectOfType<PlayerHp>();

    if (hp != null && hp.fadeImage != null)
    {
        hp.fadeImage.gameObject.SetActive(false);
    }

    if (OverImage != null)
    {
        OverImage.gameObject.SetActive(false);
    }
}

}