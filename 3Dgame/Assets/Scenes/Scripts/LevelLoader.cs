using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    private void Awake()
    {
        EnsureEventSystem();

        // 1. Bölüm oyuna başlarken her zaman açık kanka
        PlayerPrefs.SetInt("Level1_Acildi", 1);

        // Bölümleri hiyerarşideki adlarına göre hafızadan kontrol edip bağlıyoruz
        KurulumVeBaglanti("Level1", LoadLevel1);
        KurulumVeBaglanti("Level2", LoadLevel2);
        KurulumVeBaglanti("Level3", LoadLevel3);
        KurulumVeBaglanti("Level4", LoadLevel4);
        KurulumVeBaglanti("Level5", LoadLevel5);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- Sahne Yükleme Fonksiyonları ---
    public void LoadLevel1() { SeviyeKontrolVeYukle("Level1"); }
    public void LoadLevel2() { SeviyeKontrolVeYukle("Level2"); }
    public void LoadLevel3() { SeviyeKontrolVeYukle("Level3"); }
    public void LoadLevel4() { SeviyeKontrolVeYukle("Level4"); }
    public void LoadLevel5() { SeviyeKontrolVeYukle("Level5"); }

    private void SeviyeKontrolVeYukle(string levelName)
    {
        if (PlayerPrefs.GetInt(levelName + "_Acildi", 0) == 1)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning(levelName + " henüz kilitli kanka!");
        }
    }

    // --- Sadece Buton ve Yazı Yöneten Fonksiyon ---
    private void KurulumVeBaglanti(string levelName, UnityEngine.Events.UnityAction action)
    {
        Transform buttonTransform = FindChildByName(levelName);
        if (buttonTransform == null) return;

        Button button = buttonTransform.GetComponent<Button>();
        if (button == null) return;

        // Butonun altındaki senin elinle oluşturduğun o TextMeshPro yazısını buluyoruz kanka
        // (Yazının adının hiyerarşide tam olarak "LockedText" olması gerekir)
        Transform textTransform = buttonTransform.Find("LockedText");
        TextMeshProUGUI durumYazisi = null;
        if (textTransform != null)
        {
            durumYazisi = textTransform.GetComponent<TextMeshProUGUI>();
        }

        // HAFIZADAN KONTROL: Bölüm açılmış mı?
        bool isUnlocked = PlayerPrefs.GetInt(levelName + "_Acildi", 0) == 1;

        if (isUnlocked)
        {
            // 🔓 BÖLÜM AÇIKSA:
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);

            // Yazı kısmına "OYNA" yazdırabilirsin ya da tamamen boş bırakabilirsin kanka kilit kalkınca
            if (durumYazisi != null) durumYazisi.text = "";
        }
        else
        {
            // 🔒 BÖLÜM KİLİTLİYSE:
            button.interactable = false;

            // Senin elinle yerleştirdiğin yazıya dokunup "KİLİTLİ" yazdırıyoruz kanka
            if (durumYazisi != null) durumYazisi.text = "KİLİTLİ";
        }
    }

    // --- Altyapı ve Arama Fonksiyonları ---
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