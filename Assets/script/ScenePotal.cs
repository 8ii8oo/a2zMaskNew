using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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
    public bool isFade = false;

    




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
         
            filterRD.material.SetFloat(speedPropName, 0f);
            filterRD.material.SetFloat(scalePropName, 0f);
            
            filter.gameObject.SetActive(false); 
        }

        if (portalRenderer != null)
        {
            portalRenderer.enabled = false;
            
           
            Color initialColor = portalRenderer.material.color;
            initialColor.a = 0f; 
            portalRenderer.material.color = initialColor;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }


    void Update()
    {
    
        if (!isTransitioning)
        {
            CheckEnemyAndTogglePortal();
        }


        if (isPlayerInPortal && !isTransitioning)
        {
            if (Input.GetKeyDown(KeyCode.D) && portalRenderer != null && portalRenderer.enabled) 
            {
                isFade = true;
                isTransitioning = true;
                if (spinePlayer != null && spinePlayer.AnimationState != null)
                {
                    spinePlayer.AnimationState.SetAnimation(0, "idle", true);
                }
                StartCoroutine(LoadSceneDelay(playerObj));
            }
        }
    }

    void CheckEnemyAndTogglePortal()
    {
        if (portalRenderer == null) return;

      
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 

        if (enemies.Length == 0)
        {

            if (!portalRenderer.enabled)
            {
                portalRenderer.enabled = true;
        
                StartCoroutine(PortalFadeIn(1.5f)); 
            }
        }
        else
        {
          
            if (portalRenderer.enabled)
            {

                portalRenderer.enabled = false;

                StopCoroutine("PortalFadeIn"); 
            }
        }
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
            isFade = true;
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
            playerRigid.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(1f);

        float duration = 3f;
        float elapsedTime = 0f;

        float startSpeed = 0f;
        float targetSpeed = 3f;
        float startScale = 0f;
        float targetScale = 50f;

        // 필터 페이드 인
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
        if (PlayerMove.instance != null)
        {
            PlayerMove.instance.isPortal = false;

            if (PlayerMove.instance.rigid != null)
            {
                PlayerMove.instance.rigid.linearVelocity = Vector2.zero;
            }
        }
        
        StartCoroutine(RestoreEffect());

    }
    
    IEnumerator PortalFadeIn(float duration)
    {
        if (portalRenderer == null) yield break;
        isFade = true;
        
        float elapsedTime = 0f;
        Color startColor = portalRenderer.material.color;
        startColor.a = 0f; 
        Color targetColor = startColor;
        targetColor.a = 1f; 
        
        portalRenderer.material.color = startColor;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Color currentColor = Color.Lerp(startColor, targetColor, t); 
            portalRenderer.material.color = currentColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
       isFade = false;
        
        portalRenderer.material.color = targetColor; 
    }

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

        // 필터 페이드 아웃
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

    void isPotalSound()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Potal);
    }
}