using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SceneChange sceneChange;
    public GameObject EscSet; //esc눌렀을때 뜨는 팝업창
    public GameObject Ctrl;
    public GameObject Option;
    public static bool GameIsPaused = false; //상태.  일시정지, 창 켜져잇는지 아닌지 확인
    static bool CTSA = false; //SA = SetActive, 컨트롤
    static bool OPSA = false; //옵션
    bool isTutorial;
    //안된어ㅣ남어ㅏㅣㄴㅁ어ㅣㅏ머이ㅏ저인마어
//dsajlkzdsadsadsa

    


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
            AudioManager.instance.PlaySfx(AudioManager.Sfx.button);
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                pasue();
            }
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
 AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
 
    //StartCoroutine(sceneChange.FadeOutAndLoad(sceneName));
}


    public void OnClickExit()
    {
        Application.Quit();
    }
     
}
