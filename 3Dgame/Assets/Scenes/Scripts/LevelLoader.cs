using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    private void Awake()
    {
        EnsureEventSystem();
        BindLevelButton("Level1", LoadLevel1);
        BindLevelButton("Level2", LoadLevel2);
        BindLevelButton("Level3", LoadLevel3);
        BindLevelButton("Level4", LoadLevel4);
        BindLevelButton("Level5", LoadLevel5);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadLevel1()
    {
        LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        LoadScene("Level3");
    }

    public void LoadLevel4()
    {
        LoadScene("Level4");
    }

    public void LoadLevel5()
    {
        LoadScene("LevelFınal");
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
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

    private void BindLevelButton(string buttonName, UnityEngine.Events.UnityAction action)
    {
        Transform buttonTransform = FindChildByName(buttonName);
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
