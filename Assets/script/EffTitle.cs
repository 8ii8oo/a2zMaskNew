using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EffTitle : MonoBehaviour
{
    public Image tE;
    private bool isStarted = false;

    void Start()
    {
        if(tE == null)
            tE = GetComponent<Image>();

        StartCoroutine(textEffect());
    }

    void Update()
    {
        if(!isStarted && Input.anyKeyDown)
        {
            isStarted = true;
        }
    }

    IEnumerator textEffect()
    {
        while(!isStarted)
        {
            yield return StartCoroutine(FadeImage(tE, 0f, 1f, 1f)); 

            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(FadeImage(tE, 1f, 0f, 1f));


            yield return new WaitForSeconds(0.2f);
        }
        
        tE.color = new Color(1, 1, 1, 1);
    }


    IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        
        Color color = image.color;
        color.a = startAlpha;
        image.color = color;
        
        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            
            float t = elapsedTime / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            image.color = color;
            
            yield return null; 
        }

        color.a = endAlpha;
        image.color = color;
    }
}