using UnityEngine;
using UnityEngine.UI;

public class SilahVerisi : MonoBehaviour
{
    [Header("Silah Bilgileri")]
    public string silahAdi;
    public int satinAlmaFiyati = 500;
    public bool satinAlindi = false;

    [Header("UI Bağlantısı (Sadece Mağaza Sahnesindeyse Bağla)")]
    public Button satinAlmaButonu;

    void Start()
    {
        // Tabanca her zaman ve her sahnede ücretsiz ve açık olsun kanka
        if (silahAdi == "Tabanca")
        {
            satinAlindi = true;
            PlayerPrefs.SetInt(silahAdi + "_SatinAlindi", 1);
        }

        // 🎯 HAFIZADAN KONTROL: Bu silah daha önce satın alınmış mı? (1 ise evet, 0 ise hayır)
        if (PlayerPrefs.GetInt(silahAdi + "_SatinAlindi", 0) == 1)
        {
            satinAlindi = true;
        }

        ButonUIGuncelle();
    }

    public void ButonUIGuncelle()
    {
        // Eğer bu script oyun sahnesindeyse buton bağlı olmayacağı için hata vermesin diye kontrol koyduk kanka
        if (satinAlmaButonu != null)
        {
            if (satinAlindi)
            {
                satinAlmaButonu.interactable = false;
                satinAlmaButonu.GetComponentInChildren<TMPro.TMP_Text>().text = "AÇIK";
            }
            else
            {
                satinAlmaButonu.interactable = true;
                // 🎯 FIX: Fiyat yazısını tamamen kaldırdık kanka, butonun üzerinde sadece temiz bir şekilde "SATIN AL" yazacak!
                satinAlmaButonu.GetComponentInChildren<TMPro.TMP_Text>().text = "SATIN AL";
            }
        }
    }
}