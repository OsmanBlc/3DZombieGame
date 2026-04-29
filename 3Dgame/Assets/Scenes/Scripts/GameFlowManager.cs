using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private Slider musicSlider;
    private Slider sfxSlider;
    private TMP_Dropdown graphicsDropdown;

    private bool isPaused;
    private bool isGameOver;
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
        if (!gameplaySceneActive || isGameOver)
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

        settingsPanel = CreateDialog(pausePanel.transform, "SettingsPanel", "AYARLAR");
        CreateSlider(settingsPanel.transform, "Muzik", PlayerPrefs.GetFloat("MusicVolume", 1f), SetMusicVolume, out musicSlider);
        CreateSlider(settingsPanel.transform, "SFX", PlayerPrefs.GetFloat("SfxVolume", SettingsManager.SfxVolume), SetSfxVolume, out sfxSlider);
        CreateGraphicsDropdown(settingsPanel.transform);
        CreateButton(settingsPanel.transform, "Geri", ShowPauseMain);

        gameOverPanel = CreateOverlay("GameOverPanel");
        GameObject gameOverDialog = CreateDialog(gameOverPanel.transform, "GameOverDialog", "OLDUN");
        CreateButton(gameOverDialog.transform, "Tekrar Dene", RestartLevel);
        CreateButton(gameOverDialog.transform, "Ana Menu", LoadMainMenu);

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

    private GameObject CreateDialog(Transform parent, string objectName, string title)
    {
        GameObject dialog = new GameObject(objectName);
        dialog.transform.SetParent(parent, false);

        RectTransform rectTransform = dialog.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(520f, 560f);

        Image background = dialog.AddComponent<Image>();
        background.color = new Color(0.06f, 0.07f, 0.08f, 0.96f);

        VerticalLayoutGroup layout = dialog.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(42, 42, 38, 38);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateText(dialog.transform, title, 46, FontStyles.Bold, 72f);
        return dialog;
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
        image.color = new Color(0.82f, 0.08f, 0.06f, 0.94f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.98f, 0.18f, 0.12f, 1f);
        colors.pressedColor = new Color(0.55f, 0.04f, 0.04f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 66f;

        TextMeshProUGUI text = CreateText(buttonObject.transform, label, 28, FontStyles.Bold, 66f);
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
        rowLayout.spacing = 18f;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlWidth = false;
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
        sliderLayout.preferredWidth = 290f;
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
}
