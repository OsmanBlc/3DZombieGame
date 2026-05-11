using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;

    private void Awake()
    {
        EnsureEventSystem();
        FindPanelsIfMissing();
        ConfigureDecorativeImages();
        BindButtons();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CloseSettings();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenSettings()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatildi");
    }

    private void EnsureEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            StandaloneInputModule standaloneInput = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneInput == null)
                standaloneInput = eventSystem.gameObject.AddComponent<StandaloneInputModule>();

            BaseInputModule[] inputModules = eventSystem.GetComponents<BaseInputModule>();
            foreach (BaseInputModule inputModule in inputModules)
                inputModule.enabled = inputModule == standaloneInput;

            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private void FindPanelsIfMissing()
    {
        if (mainPanel == null)
        {
            Transform buttons = FindChildByName("buttons");
            if (buttons != null)
                mainPanel = buttons.gameObject;
        }

        if (settingsPanel == null)
        {
            Transform settings = FindChildByName("SettingsPanel");
            if (settings != null)
                settingsPanel = settings.gameObject;
        }
    }

    private void ConfigureDecorativeImages()
    {
        SetPanelImageRaycast(mainPanel, false);
    }

    private void SetPanelImageRaycast(GameObject panel, bool raycastTarget)
    {
        if (panel == null)
            return;

        Image image = panel.GetComponent<Image>();
        if (image != null)
            image.raycastTarget = raycastTarget;
    }

    private void BindButtons()
    {
        BindButton("StartGame", StartGame);
        BindButton("Ayarlar", OpenSettings);
        BindButton("GeriButton", CloseSettings);
        BindButton("Çıkış", ExitGame);
        BindButton("Cikis", ExitGame);
        BindButton("Cıkıs", ExitGame);
    }

    private void BindButton(string objectName, UnityEngine.Events.UnityAction action)
    {
        Transform buttonTransform = FindChildByName(objectName);
        if (buttonTransform == null)
            return;

        Button button = buttonTransform.GetComponent<Button>();
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    private Transform FindChildByName(string objectName)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform candidate in transforms)
        {
            if (candidate.name != objectName)
                continue;

            if (!candidate.gameObject.scene.IsValid())
                continue;

            return candidate;
        }

        return null;
    }
}
