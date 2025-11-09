using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;


public class PotalScene : MonoBehaviour
{
    public string sceneName;
    public GameObject filter;
    private bool isTransitioning = false;

    public Renderer filterRD;
    private string speedPropName = "_speed";
    private string scalePropName = "_scale";


    void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoad;

        if (filterRD != null)
        {
            filterRD.material.SetFloat(speedPropName, 0f);
            filterRD.material.SetFloat(scalePropName, 0f);
        }



    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadSceneDelay(other.gameObject));
        }
    }

    IEnumerator LoadSceneDelay(GameObject PlayerToStop)
    {

        filter.gameObject.SetActive(true);

        Rigidbody2D playerRigid = PlayerToStop.GetComponent<Rigidbody2D>();
        if (playerRigid != null)
        {
            playerRigid.simulated = false;
            playerRigid.linearVelocity = Vector2.zero;
        }

        PlayerMove playerMoveScript = PlayerToStop.GetComponent<PlayerMove>();
        if (playerMoveScript != null)
        {
            playerMoveScript.enabled = false;
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("점점커짐");
        float duration = 3f;
        float elapsedTime = 0f;

        float startSpeed = 0f;
        float targetSpeed = 3f;
        float startScale = 0f;
        float targetScale = 50f;


        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            float currentScale = Mathf.Lerp(startScale, targetScale, t);

                if (filterRD != null)
                {
                    filterRD.material.SetFloat(speedPropName, currentSpeed);
                    filterRD.material.SetFloat(scalePropName, currentScale);
                }

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        if (filterRD != null)
        {
            filterRD.material.SetFloat(speedPropName, targetSpeed);
            filterRD.material.SetFloat(scalePropName, targetScale);
        }

        SceneManager.LoadScene(sceneName);
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RestoreEffect());
    }
    
    IEnumerator RestoreEffect()
    {

        
        if (filterRD == null) yield break;
        filterRD.gameObject.SetActive(true);

        float duration = 3f;
        float elapsedTime = 0f;

        float startSpeed = 3f;
        float targetSpeed = 0f;
        float startScale = 50f;
        float targetScale = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            float currentScale = Mathf.Lerp(startScale, targetScale, t);

            if (filterRD != null)
            {
                filterRD.material.SetFloat(speedPropName, currentSpeed);
                filterRD.material.SetFloat(scalePropName, currentScale);
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }
                
                filterRD.material.SetFloat(speedPropName, targetSpeed);
                filterRD.material.SetFloat(scalePropName, targetScale);
        
        }
    }

