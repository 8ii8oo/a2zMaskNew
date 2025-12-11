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
        foreach (var obj in tutorialObjects)
        {
            obj.SetActive(false);
        }

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


        if (currentIndex == 0 && leftArrow && rightArrow)
        {
            leftArrow = rightArrow = false;
            StartCoroutine(SwitchToNextObject());
        }

     
        if (currentIndex == 1 && Input.GetKeyDown(KeyCode.LeftShift) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }
        if (currentIndex == 2 && Input.GetKeyDown(KeyCode.Space) && !isFading)
        {
            StartCoroutine(SwitchToNextObject());
        }

        if (currentIndex == 3 && Input.GetKey(KeyCode.DownArrow) && !isFading)
        {
            if(Input.GetKeyDown(KeyCode.Space) && !isFading)
            {
                StartCoroutine(SwitchToNextObject());
            }
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


/*
         objectsWithTag = GameObject.FindGameObjectsWithTag(targetTag);
         if(objectsWithTag.Length == 1 && !isPotalTutorialActive)
        {
            SpriteRenderer PotalRenderer = PotalTutorial.GetComponent<SpriteRenderer>();
            PotalTutorial.SetActive(true);
            StartCoroutine(FadeIn(PotalRenderer));
            isPotalTutorialActive = true;
        }
        */

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

        yield return StartCoroutine(FadeOut(currentRenderer));

        tutorialObjects[currentIndex].SetActive(false);

        currentIndex++;

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

    public void StartPotalTutorial()
{
    if(isPotalTutorialActive) return;
    
    if (currentIndex < tutorialObjects.Length)
    {
        tutorialObjects[currentIndex].SetActive(false);
    }
    

    SpriteRenderer PotalRenderer = PotalTutorial.GetComponent<SpriteRenderer>();
    if (PotalRenderer != null)
    {
        PotalTutorial.SetActive(true);
        SetAlpha(PotalRenderer, 0f);
        StartCoroutine(FadeIn(PotalRenderer));
        isPotalTutorialActive = true;
    }
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
