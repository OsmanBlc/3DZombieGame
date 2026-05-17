using System;
using UnityEngine;
using UnityEngine.UI;

public class SilahDegistirici : MonoBehaviour
{
    [Header("Silahlar (Hazırladığın 3 El+Silah Klasörünü Buraya At)")]
    public GameObject[] silahlar;
    private int aktifSilahIndex = 0;

    [Header("UI")]
    public Image silahIkonu;
    public Sprite[] silahSprite;

    [Header("Ekonomi Sistemi")]
    public int toplamPara = 0;
    public TMPro.TMP_Text paraYazisi;

    void Start()
    {
        // 🎯 HAFIZADAN YÜKLE: Oyun açıldığında daha önce kaydedilen parayı çekiyoruz kanka.
        toplamPara = PlayerPrefs.GetInt("ToplamPara", 0);

        // Bug Engelleme: İlk açılışta sadece aktif silahı açar
        if (silahlar != null && Array.Exists(silahlar, x => x != null))
        {
            for (int i = 0; i < silahlar.Length; i++)
            {
                if (silahlar[i] != null)
                    silahlar[i].SetActive(i == aktifSilahIndex);
            }
        }

        ParaUIGuncelle();
        SilahIkonunuGuncelle();
    }

    // 🎯 TEK VE GERÇEK PARAEKLE MOTORU: Hem ekler hem PlayerPrefs ile hafızaya kilitler aga
    public void ParaEkle(int miktar)
    {
        toplamPara += miktar;

        PlayerPrefs.SetInt("ToplamPara", toplamPara);
        PlayerPrefs.Save();

        ParaUIGuncelle();
    }

    void Update()
    {
        // Klavyeden silaha basınca kilit kontrolü yapıyoruz kanka
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) { SilahSec(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) { SilahSec(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) { SilahSec(2); return; }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) ScrollButonYonu(1);
        else if (scroll < 0f) ScrollButonYonu(-1);
    }

    void ScrollButonYonu(int yon)
    {
        int hedefIndex = aktifSilahIndex + yon;

        // Sınır kontrolleri döngüsü
        if (hedefIndex >= silahlar.Length) hedefIndex = 0;
        else if (hedefIndex < 0) hedefIndex = silahlar.Length - 1;

        SilahSec(hedefIndex);
    }

    void SilahSec(int yeniSilahIndex)
    {
        if (silahlar == null || silahlar.Length == 0) return;

        // 🛡️ KİLİTLİ SİLAH DUVARI: Geçiş yapılmak istenen silah satın alınmadıysa geçişi iptal et!
        SilahVerisi hedefSilahVerisi = silahlar[yeniSilahIndex].GetComponentInChildren<SilahVerisi>();
        if (hedefSilahVerisi != null && !hedefSilahVerisi.satinAlindi)
        {
            Debug.LogWarning(hedefSilahVerisi.silahAdi + " kilitli kanka! Önce mağazadan satın al.");
            return;
        }

        // Sinsi Reload Engelleme (Şarjör değiştirirken silah değiştirmeyi kapatır)
        if (silahlar[aktifSilahIndex] != null)
        {
            SilahAtes suAnkiSilahScripti = silahlar[aktifSilahIndex].GetComponentInChildren<SilahAtes>();
            if (suAnkiSilahScripti != null && suAnkiSilahScripti.ReloadYapiyorMu()) return;
        }

        if (aktifSilahIndex == yeniSilahIndex && silahlar[aktifSilahIndex].activeSelf) return;

        aktifSilahIndex = yeniSilahIndex;
        SilahlariGuncelle();
    }

    void SilahlariGuncelle()
    {
        for (int i = 0; i < silahlar.Length; i++)
        {
            if (silahlar[i] != null)
                silahlar[i].SetActive(i == aktifSilahIndex);
        }
        SilahIkonunuGuncelle();
    }

    // 🎯 MAĞAZADAN SATIN ALMA FONKSİYONU
    public void SilahSatinAl(int silahIndex)
    {
        SilahVerisi veri = silahlar[silahIndex].GetComponentInChildren<SilahVerisi>();

        if (veri == null) return;
        if (veri.satinAlindi) return;

        if (toplamPara >= veri.satinAlmaFiyati)
        {
            toplamPara -= veri.satinAlmaFiyati;
            veri.satinAlindi = true;
            veri.ButonUIGuncelle();

            // 🎯 HARCANAN PARAYI DA HAFIZAYA KİLİTLİYORUZ
            PlayerPrefs.SetInt("ToplamPara", toplamPara);
            PlayerPrefs.Save();

            ParaUIGuncelle();
            Debug.Log(veri.silahAdi + " başarıyla satın alındı kanka!");
        }
        else
        {
            Debug.LogWarning("Paran yetmiyor kanka! Zombi avlamaya devam.");
        }
    }

    void ParaUIGuncelle()
    {
        if (paraYazisi != null)
            paraYazisi.text = toplamPara.ToString(); // Sadece saf sayı kanka, "TL" yok
    }

    void SilahIkonunuGuncelle()
    {
        if (silahIkonu != null && silahSprite != null && silahSprite.Length > aktifSilahIndex)
        {
            silahIkonu.sprite = silahSprite[aktifSilahIndex];

            if (aktifSilahIndex == 0)
                silahIkonu.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            else if (aktifSilahIndex == 1)
                silahIkonu.rectTransform.localScale = new Vector3(0.65f, 0.65f, 1f);
            else if (aktifSilahIndex == 2)
                silahIkonu.rectTransform.localScale = new Vector3(0.60f, 0.60f, 1f);
        }
    }
}