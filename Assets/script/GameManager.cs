
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameClearPanel;
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

    void Awake()
{
    if(instance == null)
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    else
    {
        Destroy(gameObject);
        return;
    }
}




    void Start()
    {
    

        
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


    public void OnClickRePlay()
{

     gameObject.SetActive(true);

    AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
    Time.timeScale = 1f;

    string nextScene = GameManager.lastSceneName == "tutorial" ? "tutorial" : "Stage11";

    if (sceneChange != null)
{
    sceneChange.sceneName = nextScene;
    sceneChange.StartCoroutine(sceneChange.FadeThenClearUIAndLoad(this));
}
    else
    {
        SceneManager.LoadScene(nextScene);
        StartCoroutine(ResetPlayerSkinAfterSceneLoad());
    }
}





IEnumerator ResetPlayerSkinAfterSceneLoad() //노멀스킬로 초기화
{
    yield return new WaitForSeconds(0.1f); // 씬이 로드 시간

    PlayerMove player = FindObjectOfType<PlayerMove>();
    if (player != null)
    {
        player.ResetSkinToNormal();
    }
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
     if (gameClearPanel != null)
    {
        var cg = gameClearPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        gameClearPanel.SetActive(false);  
    }
    if (scene.name != "Title")
    {
        if (gameObject != null && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    scenePotal = FindObjectOfType<ScenePotal>();

    if (scene.name == "Stage11" || scene.name == "tutorial")
    {
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.ResetSkinToNormal();

            // 위치 초기화
            GameObject spawn = GameObject.FindWithTag("SpawnPoint");
            if (spawn != null)
                player.transform.position = spawn.transform.position;
        }
    }
}

public void ClearGameOverUI()
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