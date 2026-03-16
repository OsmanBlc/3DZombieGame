using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    public Slider sfxSlider;
    public Slider musicSlider;
    public TMP_Dropdown graphicsDropdown;

    [Header("Audio")]
    public AudioSource musicSource;

    public static float SfxVolume = 1f;

    private void Start()
    {
        // Eğer inspector'dan bağlanmadıysa MusicManager üstündeki AudioSource'u bul
        if (musicSource == null)
        {
            MusicManager musicManager = FindObjectOfType<MusicManager>();
            if (musicManager != null)
            {
                musicSource = musicManager.GetComponent<AudioSource>();
            }
        }

        // Slider eventleri
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        if (graphicsDropdown != null)
            graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);

        // Kaydedilmiş ayarları yükle
        LoadSettings();
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
            Debug.Log("Müzik sesi değişti: " + value);
        }

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;

        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.Save();
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);

        PlayerPrefs.SetInt("GraphicsQuality", index);
        PlayerPrefs.Save();

        Debug.Log("Kalite değişti: " + index);

        if (index == 0) // Düşük
        {
            Application.targetFrameRate = 30;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.lodBias = 0.5f;
            QualitySettings.antiAliasing = 0;
        }
        else if (index == 1) // Orta
        {
            Application.targetFrameRate = 60;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.lodBias = 1f;
            QualitySettings.antiAliasing = 2;
        }
        else if (index == 2) // Yüksek
        {
            Application.targetFrameRate = 120;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.lodBias = 2f;
            QualitySettings.antiAliasing = 4;
        }
    }

    private void LoadSettings()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSfx = PlayerPrefs.GetFloat("SfxVolume", 1f);
        int savedGraphics = PlayerPrefs.GetInt("GraphicsQuality", 0);

        if (musicSlider != null)
            musicSlider.value = savedMusic;

        if (sfxSlider != null)
            sfxSlider.value = savedSfx;

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = savedGraphics;
            graphicsDropdown.RefreshShownValue();
        }

        if (musicSource != null)
            musicSource.volume = savedMusic;

        SfxVolume = savedSfx;

        SetGraphicsQuality(savedGraphics);
    }
}