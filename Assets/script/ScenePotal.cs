using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;
using Spine.Unity;

public class ScenePotal : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation spinePlayer;
    public string sceneName;
    public GameObject filter;
    private bool isTransitioning = false;

    private bool isPlayerInPortal = false;
    private GameObject playerObj;

    public Renderer filterRD;
    private string speedPropName = "_speed";
    private string scalePropName = "_scale";
    
    private Renderer portalRenderer; 

    public GameObject[] currentEnemy;


    void Start()
    {
        portalRenderer = GetComponent<Renderer>();
        
        DontDestroyOnLoad(gameObject);
        
        if (filter != null)
        {
            DontDestroyOnLoad(filter);
        }

        SceneManager.sceneLoaded += OnSceneLoad;

        if (filterRD != null)
        {
            filterRD.sharedMaterial.SetFloat(speedPropName, 0f);
            filterRD.material.SetFloat(scalePropName, 0f);
            
            filter.gameObject.SetActive(false); 
        }
    }

    void Update()
    {
        if (isPlayerInPortal && !isTransitioning)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                isTransitioning = true;
                if (spinePlayer != null && spinePlayer.AnimationState != null)
                {
                    
                    spinePlayer.AnimationState.SetAnimation(0, "idle", true);

                }
                StartCoroutine(LoadSceneDelay(playerObj));
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
            playerObj = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
            playerObj = null;
        }
    }

    IEnumerator LoadSceneDelay(GameObject PlayerToStop)
    {
        if (filter != null)
        {
            
            filter.gameObject.SetActive(true);

            
        }

        Invoke("isPotalSound", 2.5f);
        PlayerMove playerMoveScript = PlayerToStop.GetComponent<PlayerMove>();
if (playerMoveScript != null)
{
    playerMoveScript.isPortal = true;  
}

Rigidbody2D playerRigid = PlayerToStop.GetComponent<Rigidbody2D>();
if (playerRigid != null)
{
    playerRigid.linearVelocity = Vector2.zero;  // 이동만 멈춤

}

        yield return new WaitForSeconds(1f);

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
                filterRD.sharedMaterial.SetFloat(speedPropName, currentSpeed);
                filterRD.sharedMaterial.SetFloat(scalePropName, currentScale);
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        if (filterRD != null)
        {
            filterRD.sharedMaterial.SetFloat(speedPropName, targetSpeed);
            filterRD.sharedMaterial.SetFloat(scalePropName, targetScale);
        }

        
        

        SceneManager.LoadScene(sceneName);
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "title")
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            Destroy(gameObject);
            return;
        }
        
        if (portalRenderer != null)
        {
            portalRenderer.enabled = false;
        }
        
        Camera mainCam = Camera.main;
        if (mainCam != null && filter != null)
        {
            filter.transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, mainCam.nearClipPlane + 0.1f);
        }

        isTransitioning = false;
        StartCoroutine(RestoreEffect());


        IEnumerator RestoreEffect()
        {
            if (filterRD == null || filter == null) yield break;
            
            filter.gameObject.SetActive(true);

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
                    filterRD.sharedMaterial.SetFloat(speedPropName, currentSpeed);
                    filterRD.sharedMaterial.SetFloat(scalePropName, currentScale);
                }

                elapsedTime += Time.deltaTime;
                yield return null;

            }
            

            filterRD.sharedMaterial.SetFloat(speedPropName, targetSpeed); 
            filterRD.sharedMaterial.SetFloat(scalePropName, targetScale); 
            
            filter.gameObject.SetActive(false); 

            SceneManager.sceneLoaded -= OnSceneLoad;
            Destroy(gameObject); 
        }
    } 

    void isPotalSound()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Potal);
    }
}