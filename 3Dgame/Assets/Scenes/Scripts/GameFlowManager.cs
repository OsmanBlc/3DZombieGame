using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";

    private static GameFlowManager instance;
    private static bool bootstrapped;

    private Canvas canvas;
    private GameObject pausePanel;
    private GameObject pauseMainPanel;
    private GameObject settingsPanel;
    private GameObject gameOverPanel;
    private GameObject levelCompletePanel;
    private TextMeshProUGUI yildizYazisi;
    private TextMeshProUGUI sonucYazisi;
    private TextMeshProUGUI sureYazisi;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Slider sensitivitySlider;
    private TMP_Dropdown graphicsDropdown;

    private bool isPaused;
    private bool isGameOver;
    private bool isLevelComplete;
    private bool gameplaySceneActive;
    private CursorLockMode previousLockMode;
    private bool previousCursorVisible;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (bootstrapped)
            return;

        bootstrapped = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureInstance().RefreshForScene();
    }

    public static void ShowGameOver()
    {
        EnsureInstance().OpenGameOver();
    }

    public static void ShowLevelComplete(float gecenSure, float ucYildizSure, float ikiYildizSure)
    {
        EnsureInstance().OpenLevelComplete(gecenSure, ucYildizSure, ikiYildizSure);
    }

    private static GameFlowManager EnsureInstance()
    {
        if (instance != null)
            return instance;

        GameObject managerObject = new GameObject("GameFlowManager");
        instance = managerObject.AddComponent<GameFlowManager>();
        DontDestroyOnLoad(managerObject);
        return instance;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureInstance().RefreshForScene();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!gameplaySceneActive || isGameOver || isLevelComplete)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                OpenPause();
            else if (settingsPanel != null && settingsPanel.activeSelf)
                ShowPauseMain();
            else
                ResumeGame();
        }
    }

    private void RefreshForScene()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isLevelComplete = false;
        gameplaySceneActive = FindObjectOfType<PlayerHealth>() != null;

        if (!gameplaySceneActive)
        {
            HidePanels();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        EnsureCanvas();
        HidePanels();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EnsureCanvas()
    {
        if (canvas != null)
            return;

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject esObject = new GameObject("EventSystem");
            DontDestroyOnLoad(esObject);
            esObject.AddComponent<EventSystem>();
            esObject.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasObject = new GameObject("Game Flow Canvas");
        DontDestroyOnLoad(canvasObject);

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        pausePanel = CreateOverlay("PausePanel");
        pauseMainPanel = CreateDialog(pausePanel.transform, "PauseMainPanel", "DURAKLATILDI");
        CreateButton(pauseMainPanel.transform, "Devam Et", ResumeGame);
        CreateButton(pauseMainPanel.transform, "Ayarlar", OpenSettings);
        CreateButton(pauseMainPanel.transform, "Ana Menu", LoadMainMenu);

        settingsPanel = CreateDialog(pausePanel.transform, "SettingsPanel", "AYARLAR", 560f);
        CreateSlider(settingsPanel.transform, "Muzik", PlayerPrefs.GetFloat("MusicVolume", 1f), SetMusicVolume, out musicSlider);
        CreateSlider(settingsPanel.transform, "SFX", PlayerPrefs.GetFloat("SfxVolume", SettingsManager.SfxVolume), SetSfxVolume, out sfxSlider);
        float sensNorm = Mathf.InverseLerp(30f, 300f, PlayerPrefs.GetFloat("MouseSensitivity", 150f));
        CreateSlider(settingsPanel.transform, "Hassasiyet", sensNorm, SetSensitivity, out sensitivitySlider);
        CreateGraphicsDropdown(settingsPanel.transform);
        CreateButton(settingsPanel.transform, "Geri", ShowPauseMain);

        gameOverPanel = CreateOverlay("GameOverPanel");
        GameObject gameOverDialog = CreateDialog(gameOverPanel.transform, "GameOverDialog", "GAME OVER");
        CreateText(gameOverDialog.transform, "Zombiler seni yedi...", 20, FontStyles.Italic, 32f);
        CreateButton(gameOverDialog.transform, "Tekrar Dene", RestartLevel);
        CreateButton(gameOverDialog.transform, "Ana Menu", LoadMainMenu);

        levelCompletePanel = CreateOverlay("LevelCompletePanel");
        GameObject lcDialog = CreateDialog(levelCompletePanel.transform, "LevelCompleteDialog", "BÖLÜM BİTTİ", 540f);
        yildizYazisi = CreateText(lcDialog.transform, "★★★", 64, FontStyles.Bold, 88f);
        yildizYazisi.color = new Color(1f, 0.85f, 0.1f, 1f);
        sonucYazisi = CreateText(lcDialog.transform, "", 22, FontStyles.Italic, 36f);
        sureYazisi = CreateText(lcDialog.transform, "", 20, FontStyles.Normal, 30f);
        sureYazisi.color = new Color(0.7f, 0.8f, 0.9f, 1f);
        CreateButton(lcDialog.transform, "Sonraki Bölüm", LoadNextLevel);
        CreateButton(lcDialog.transform, "Ana Menu", LoadMainMenu);

        HidePanels();
    }

    private GameObject CreateOverlay(string objectName)
    {
        GameObject overlay = new GameObject(objectName);
        overlay.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = overlay.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = overlay.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.72f);
        return overlay;
    }

    private GameObject CreateDialog(Transform parent, string objectName, string title, float height = 420f)
    {
        GameObject dialog = new GameObject(objectName);
        dialog.transform.SetParent(parent, false);

        RectTransform rectTransform = dialog.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(520f, height);

        Image borderImage = dialog.AddComponent<Image>();
        borderImage.color = new Color(0.80f, 0.07f, 0.05f, 1f);

        GameObject innerPanel = new GameObject("InnerPanel");
        innerPanel.transform.SetParent(dialog.transform, false);
        RectTransform innerRect = innerPanel.AddComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(4f, 4f);
        innerRect.offsetMax = new Vector2(-4f, -4f);
        Image background = innerPanel.AddComponent<Image>();
        background.color = new Color(0.09f, 0.10f, 0.12f, 1f);

        VerticalLayoutGroup layout = innerPanel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(40, 40, 32, 32);
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateText(innerPanel.transform, title, 42, FontStyles.Bold, 64f);
        return innerPanel;
    }

    private TextMeshProUGUI CreateText(Transform parent, string value, int fontSize, FontStyles fontStyle, float height)
    {
        GameObject textObject = new GameObject(value + " Text");
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        return text;
    }

    private Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(label + " Button");
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.22f, 0.24f, 0.28f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.88f, 0.10f, 0.08f, 1f);
        colors.pressedColor = new Color(0.50f, 0.04f, 0.04f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 58f;

        TextMeshProUGUI text = CreateText(buttonObject.transform, label, 26, FontStyles.Bold, 58f);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    private void CreateSlider(Transform parent, string label, float value, UnityEngine.Events.UnityAction<float> action, out Slider slider)
    {
        GameObject row = new GameObject(label + " Row");
        row.transform.SetParent(parent, false);

        HorizontalLayoutGroup rowLayout = row.AddComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 14f;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = false;

        LayoutElement rowLayoutElement = row.AddComponent<LayoutElement>();
        rowLayoutElement.preferredHeight = 54f;

        TextMeshProUGUI rowText = CreateText(row.transform, label, 24, FontStyles.Bold, 54f);
        LayoutElement textLayout = rowText.GetComponent<LayoutElement>();
        textLayout.preferredWidth = 120f;

        GameObject sliderObject = new GameObject(label + " Slider");
        sliderObject.transform.SetParent(row.transform, false);

        slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = value;
        slider.onValueChanged.AddListener(action);

        LayoutElement sliderLayout = sliderObject.AddComponent<LayoutElement>();
        sliderLayout.minWidth = 200f;
        sliderLayout.preferredWidth = 280f;
        sliderLayout.flexibleWidth = 1f;
        sliderLayout.preferredHeight = 28f;

        Image background = CreateSliderImage(sliderObject.transform, "Background", new Color(0.22f, 0.22f, 0.24f, 1f));
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 0.35f);
        backgroundRect.anchorMax = new Vector2(1f, 0.65f);
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        RectTransform fillArea = CreateRect(sliderObject.transform, "Fill Area");
        fillArea.anchorMin = new Vector2(0f, 0.35f);
        fillArea.anchorMax = new Vector2(1f, 0.65f);
        fillArea.offsetMin = Vector2.zero;
        fillArea.offsetMax = Vector2.zero;

        Image fill = CreateSliderImage(fillArea, "Fill", new Color(0.86f, 0.1f, 0.08f, 1f));
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        RectTransform handleArea = CreateRect(sliderObject.transform, "Handle Slide Area");
        handleArea.anchorMin = Vector2.zero;
        handleArea.anchorMax = Vector2.one;
        handleArea.offsetMin = Vector2.zero;
        handleArea.offsetMax = Vector2.zero;

        Image handle = CreateSliderImage(handleArea, "Handle", Color.white);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(28f, 28f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
    }

    private Image CreateSliderImage(Transform parent, string objectName, Color color)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private RectTransform CreateRect(Transform parent, string objectName)
    {
        GameObject rectObject = new GameObject(objectName);
        rectObject.transform.SetParent(parent, false);
        return rectObject.AddComponent<RectTransform>();
    }

    private void CreateGraphicsDropdown(Transform parent)
    {
        GameObject dropdownObject = new GameObject("Graphics Dropdown");
        dropdownObject.transform.SetParent(parent, false);

        Image image = dropdownObject.AddComponent<Image>();
        image.color = new Color(0.16f, 0.17f, 0.19f, 1f);

        graphicsDropdown = dropdownObject.AddComponent<TMP_Dropdown>();
        graphicsDropdown.options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Dusuk"),
            new TMP_Dropdown.OptionData("Orta"),
            new TMP_Dropdown.OptionData("Yuksek")
        };
        graphicsDropdown.value = Mathf.Clamp(PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel()), 0, 2);
        graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);

        LayoutElement layoutElement = dropdownObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 58f;

        TextMeshProUGUI label = CreateText(dropdownObject.transform, "Grafik Kalitesi", 24, FontStyles.Bold, 58f);
        graphicsDropdown.captionText = label;
        graphicsDropdown.RefreshShownValue();
    }

    private void HidePanels()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    private void OpenPause()
    {
        EnsureCanvas();
        previousLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pausePanel.SetActive(true);
        ShowPauseMain();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Cursor.lockState = previousLockMode == CursorLockMode.None ? CursorLockMode.Locked : previousLockMode;
        Cursor.visible = previousCursorVisible && previousLockMode != CursorLockMode.Locked;
    }

    private void OpenSettings()
    {
        pauseMainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void ShowPauseMain()
    {
        pauseMainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OpenGameOver()
    {
        EnsureCanvas();
        isGameOver = true;
        isPaused = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        gameOverPanel.SetActive(true);
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.buildIndex >= 0)
            SceneManager.LoadScene(currentScene.buildIndex);
        else
            SceneManager.LoadScene(currentScene.name);
    }

    private void OpenLevelComplete(float gecenSure, float ucYildizSure, float ikiYildizSure)
    {
        EnsureCanvas();
        isLevelComplete = true;
        isPaused = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        string yildiz;
        string sonuc;
        if (gecenSure < ucYildizSure)
        {
            yildiz = "★★★";
            sonuc = "Mükemmel! Çok hızlısın!";
        }
        else if (gecenSure < ikiYildizSure)
        {
            yildiz = "★★☆";
            sonuc = "İyi iş!";
        }
        else
        {
            yildiz = "★☆☆";
            sonuc = "Bitirdin en azından!";
        }

        if (yildizYazisi != null)
        {
            yildizYazisi.text = yildiz;
            yildizYazisi.color = new Color(1f, 0.85f, 0.1f, 1f);
        }

        if (sonucYazisi != null)
            sonucYazisi.text = sonuc;

        if (sureYazisi != null)
            sureYazisi.text = "Süre: " + Mathf.RoundToInt(gecenSure) + " saniye";

        levelCompletePanel.SetActive(true);
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager == null)
            return;

        AudioSource musicSource = musicManager.GetComponent<AudioSource>();
        if (musicSource != null)
            musicSource.volume = value;
    }

    private void SetSfxVolume(float value)
    {
        SettingsManager.SfxVolume = value;
        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.Save();
    }

    private void SetGraphicsQuality(int index)
    {
        index = Mathf.Clamp(index, 0, 2);
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt("GraphicsQuality", index);
        PlayerPrefs.Save();
    }

    private void SetSensitivity(float value)
    {
        float sensitivity = Mathf.Lerp(30f, 300f, value);
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.mouseSensitivity = sensitivity;
    }
}
