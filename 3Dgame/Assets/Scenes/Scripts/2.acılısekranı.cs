using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 3f;

    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        SetFadeAlpha(1f); // başta tamamen siyah

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);
    }

    void SetFadeAlpha(float alpha)
    {
        Color color = fadePanel.color;
        color.a = alpha;
        fadePanel.color = color;
    }
}