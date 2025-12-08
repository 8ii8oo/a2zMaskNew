using System.Collections;
using UnityEngine;

public class tutorial : MonoBehaviour
{
    public GameObject[] tutorialObjects; // 튜토리얼용 오브젝트들 (순서대로)
    private int currentIndex = 0;

    public float fadeTime = 1f;

    private bool isPotalTutorialActive = false;

    private SpriteRenderer currentRenderer;

    private bool isFading = false;
    private bool leftArrow = false;
    private bool rightArrow = false;

    string targetTag = "Potal";
    public GameObject PotalTutorial;    
    GameObject[] objectsWithTag;

    void Start()
    {
        // 모든 오브젝트 비활성화
        foreach (var obj in tutorialObjects)
        {
            obj.SetActive(false);
        }

        // 첫 번째 오브젝트 활성화 + 페이드 인
        ActivateObject(currentIndex);
    }

    void Update()
    {
        if (isFading) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftArrow = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightArrow = true;
        }

        // 첫 번째 단계는 좌우 입력 둘 다 필요
        if (currentIndex == 0 && leftArrow && rightArrow)
        {
            leftArrow = rightArrow = false;
            StartCoroutine(SwitchToNextObject());
        }

        // 나머지 단계는 스페이스 바로 넘어감
        if (currentIndex == 1 && Input.GetKeyDown(KeyCode.Space) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }

        if (currentIndex == 2 && Input.GetKey(KeyCode.DownArrow) && !isFading)
        {
            if(Input.GetKeyDown(KeyCode.Space) && !isFading)
            {
                StartCoroutine(SwitchToNextObject());
            }
        }

        if (currentIndex == 3 && Input.GetKeyDown(KeyCode.LeftShift) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }

        if (currentIndex == 4 && Input.GetKeyDown(KeyCode.Q) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }

        if (currentIndex == 5 && Input.GetKeyDown(KeyCode.A) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }

        if (currentIndex == 6 && Input.GetKeyDown(KeyCode.S) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }



         objectsWithTag = GameObject.FindGameObjectsWithTag(targetTag);
         if(objectsWithTag.Length == 1 && !isPotalTutorialActive)
        {
            SpriteRenderer PotalRenderer = PotalTutorial.GetComponent<SpriteRenderer>();
            PotalTutorial.SetActive(true);
            StartCoroutine(FadeIn(PotalRenderer));
            isPotalTutorialActive = true;
        }

    }


    void ActivateObject(int index)
    {
        if (index >= tutorialObjects.Length) return;

        GameObject obj = tutorialObjects[index];
        obj.SetActive(true);
        currentRenderer = obj.GetComponent<SpriteRenderer>();
        SetAlpha(currentRenderer, 0f);
        StartCoroutine(FadeIn(currentRenderer));
    }

    IEnumerator SwitchToNextObject()
    {
        isFading = true;

        // 현재 오브젝트 페이드 아웃
        yield return StartCoroutine(FadeOut(currentRenderer));

        // 현재 오브젝트 비활성화
        tutorialObjects[currentIndex].SetActive(false);

        // 인덱스 증가
        currentIndex++;

        // 다음 오브젝트가 있다면 페이드 인
        if (currentIndex < tutorialObjects.Length)
        {
            ActivateObject(currentIndex);
        }

        isFading = false;
    }

    IEnumerator FadeIn(SpriteRenderer sr)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeTime;
            SetAlpha(sr, alpha);
            yield return null;
        }
        SetAlpha(sr, 1f);
    }

    IEnumerator FadeOut(SpriteRenderer sr)
    {
        yield return new WaitForSeconds(0.5f);
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeTime;
            SetAlpha(sr, alpha);
            yield return null;
        }
        SetAlpha(sr, 0f);
    }

    void SetAlpha(SpriteRenderer sr, float alpha)
    {
        Color c = sr.color;
        c.a = Mathf.Clamp01(alpha);
        sr.color = c;
    }
}
