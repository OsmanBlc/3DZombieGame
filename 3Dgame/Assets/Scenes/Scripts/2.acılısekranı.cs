using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 3f;

    private void Start()
    {
        EnsureFadePanel();

        if (fadePanel == null)
        {
            Debug.LogWarning("SceneFadeIn: fade panel bulunamadi, fade efekti atlandi.", this);
            return;
        }

        StartCoroutine(FadeInRoutine());
    }

    private void EnsureFadePanel()
    {
        if (fadePanel == null)
            fadePanel = GetComponentInChildren<Image>(true);

        if (fadePanel == null)
            fadePanel = CreateFadePanel();

        if (fadePanel != null)
        {
            fadePanel.raycastTarget = false;
            fadePanel.gameObject.SetActive(true);
        }
    }

    private Image CreateFadePanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
            return null;

        GameObject panelObject = new GameObject("SceneFadePanel");
        panelObject.transform.SetParent(canvas.transform, false);
        panelObject.transform.SetAsLastSibling();

        RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = panelObject.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;
        return image;
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
        fadePanel.gameObject.SetActive(false);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadePanel == null)
            return;

        Color color = fadePanel.color;
        color.a = alpha;
        fadePanel.color = color;
    }
}
