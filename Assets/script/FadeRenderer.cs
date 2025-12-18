using System.Collections;
using UnityEngine;

public class FadeRenderer : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private float defaultDuration = 1.5f;

    Coroutine fadeRoutine;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
    }

    public void FadeIn()
    {
        Fade(0f, 1f, defaultDuration);
    }

    public void FadeOut()
    {
        Fade(1f, 0f, defaultDuration);
    }

    public void Fade(float from, float to, float duration)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCoroutine(from, to, duration));
    }

    IEnumerator FadeCoroutine(float from, float to, float duration)
    {
        float elapsed = 0f;

        SetAlpha(from);
        targetRenderer.enabled = true;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            SetAlpha(Mathf.Lerp(from, to, t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(to);

        if (to == 0f)
            targetRenderer.enabled = false;
    }

    void SetAlpha(float alpha)
    {
        Color c = targetRenderer.material.color;
        c.a = alpha;
        targetRenderer.material.color = c;
    }
}
