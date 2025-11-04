using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject EscSet; //esc눌렀을때 뜨는 팝업창
    public static bool GameIsPaused = false; //상태.  일시정지, 창 켜져잇는지 아닌지 확인


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
        EscSet.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    
    
     
}
