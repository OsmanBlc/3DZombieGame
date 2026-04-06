using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public Slider loadingBar;
    public Image fadePanel;
    public string nextSceneName = "MainScene";

    public float loadingDuration = 5f; // barın dolma süresi
    public float fadeDuration = 3f;    // kararma süresi

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        float time = 0f;

        // Fade panel başta görünmesin
        SetFadeAlpha(0f);

        // Loading bar doluyor
        while (time < loadingDuration)
        {
            time += Time.deltaTime;
            float progress = time / loadingDuration;
            loadingBar.value = progress;
            yield return null;
        }

        loadingBar.value = 1f;

        yield return new WaitForSeconds(0.5f);
        
        loadingBar.gameObject.SetActive(false);

        // Ekran 3 saniyede kararsın
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Yeni sahneye geç
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }

    void SetFadeAlpha(float alpha)
    {
        Color color = fadePanel.color;
        color.a = alpha;
        fadePanel.color = color;
    }
}