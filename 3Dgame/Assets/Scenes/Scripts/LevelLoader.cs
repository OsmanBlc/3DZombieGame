using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private Vector2 lockedIconSize = new Vector2(110f, 110f);
    [SerializeField] private float lockedIconDuration = 1.5f;

    private Coroutine lockedIconRoutine;
    private Image activeLockedIcon;

    private void Awake()
    {
        EnsureEventSystem();
        BindLevelButton("Level1", LoadLevel1);
        BindLevelButton("Level2", () => ShowLockedIcon("Level2"));
        BindLevelButton("Level3", () => ShowLockedIcon("Level3"));
        BindLevelButton("Level4", () => ShowLockedIcon("Level4"));
        BindLevelButton("Level5", () => ShowLockedIcon("Level5"));
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
        ShowLockedIcon("Level2");
    }

    public void LoadLevel3()
    {
        ShowLockedIcon("Level3");
    }

    public void LoadLevel4()
    {
        ShowLockedIcon("Level4");
    }

    public void LoadLevel5()
    {
        ShowLockedIcon("Level5");
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void ShowLockedIcon(string buttonName)
    {
        Transform buttonTransform = FindChildByName(buttonName);
        if (buttonTransform == null)
        {
            Debug.Log("Kilitli");
            return;
        }

        Image lockedImage = GetOrCreateLockedIcon(buttonTransform);

        if (activeLockedIcon != null && activeLockedIcon != lockedImage)
            activeLockedIcon.gameObject.SetActive(false);

        activeLockedIcon = lockedImage;
        lockedImage.sprite = lockedIcon;
        lockedImage.gameObject.SetActive(true);

        if (lockedIconRoutine != null)
            StopCoroutine(lockedIconRoutine);

        lockedIconRoutine = StartCoroutine(HideLockedIconAfterDelay(lockedImage));
    }

    private System.Collections.IEnumerator HideLockedIconAfterDelay(Image lockedImage)
    {
        yield return new WaitForSecondsRealtime(lockedIconDuration);

        if (lockedImage != null)
            lockedImage.gameObject.SetActive(false);

        if (activeLockedIcon == lockedImage)
            activeLockedIcon = null;

        lockedIconRoutine = null;
    }

    private Image GetOrCreateLockedIcon(Transform buttonTransform)
    {
        Transform existingIcon = buttonTransform.Find("LockedIcon");
        if (existingIcon != null && existingIcon.TryGetComponent(out Image image))
            return image;

        GameObject lockedIconObject = new GameObject("LockedIcon");
        lockedIconObject.transform.SetParent(buttonTransform, false);

        RectTransform rectTransform = lockedIconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = lockedIconSize;

        Image lockedImage = lockedIconObject.AddComponent<Image>();
        lockedImage.sprite = lockedIcon;
        lockedImage.preserveAspect = true;
        lockedImage.raycastTarget = false;

        Shadow shadow = lockedIconObject.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2f, -2f);

        return lockedImage;
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
