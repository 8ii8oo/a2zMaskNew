using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject EscSet; //esc눌렀을때 뜨는 팝업창
    public GameObject Ctrl;
    public GameObject Option;
    public static bool GameIsPaused = false; //상태.  일시정지, 창 켜져잇는지 아닌지 확인
    static bool CTSA = false; //SA = SetActive, 컨트롤
    static bool OPSA = false; //옵션

    


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
        EscSet.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    void Resume()
    {
        if (CTSA == true)
        {
            Ctrl.SetActive(false);
            CTSA = false;
            return;
        }
        if(OPSA == true)
        {
            Option.SetActive(false);
            OPSA = false;
            return;
        }
        EscSet.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void OnClickCtrl()
    {
        Ctrl.SetActive(!Ctrl.activeSelf);

        CTSA = Ctrl.activeSelf;
    }

    public void OnClickOption()
    {
        Option.SetActive(!Option.activeSelf);

        OPSA = Option.activeSelf;
    }




    public void OnClickReturn()
    {
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
    
    public void OnClickTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("title");
    }
     
}
