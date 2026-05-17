using UnityEngine;
using TMPro;
using System.Collections;

public class MagazaManager : MonoBehaviour
{
    [Header("Ekonomi UI")]
    public TMP_Text magazaParaYazisi;

    [Header("Bildirim Sistemi")]
    [Tooltip("Mağaza ekranının ortasında çıkacak uyarı metni")]
    public TMP_Text bildirimYazisiText;

    [Header("Silah Buton Ayarları")]
    public int smgFiyati = 1500;
    public int pompaliFiyati = 3000;

    public SilahVerisi magazaSmgVerisi;
    public SilahVerisi magazaPompaliVerisi;

    private int toplamPara;

    void Start()
    {
        toplamPara = PlayerPrefs.GetInt("ToplamPara", 0);
        ParaUIGuncelle();

        // Oyun ilk açıldığında bildirim yazısı gizli olsun kanka
        if (bildirimYazisiText != null)
            bildirimYazisiText.gameObject.SetActive(false);
    }

    public void SMGSatinAl()
    {
        if (PlayerPrefs.GetInt("SMG_SatinAlindi", 0) == 1)
        {
            BildirimGoster("BU SİLAHA ZATEN SAHİPSİN!", Color.yellow);
            return;
        }

        if (toplamPara >= smgFiyati)
        {
            toplamPara -= smgFiyati;

            PlayerPrefs.SetInt("ToplamPara", toplamPara);
            PlayerPrefs.SetInt("SMG_SatinAlindi", 1);
            PlayerPrefs.Save();

            if (magazaSmgVerisi != null)
            {
                magazaSmgVerisi.satinAlindi = true;
                magazaSmgVerisi.ButonUIGuncelle();
            }

            ParaUIGuncelle();
            // 🎯 BAŞARILI BİLDİRİMİ
            BildirimGoster("SMG BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
            // ❌ YETERSİZ BAKİYE BİLDİRİMİ
            BildirimGoster("YETERSİZ BAKİYE! ZOMBİ AVLA.", Color.red);
        }
    }

    public void PompaliSatinAl()
    {
        if (PlayerPrefs.GetInt("Pompali_SatinAlindi", 0) == 1)
        {
            BildirimGoster("BU SİLAHA ZATEN SAHİPSİN!", Color.yellow);
            return;
        }

        if (toplamPara >= pompaliFiyati)
        {
            toplamPara -= pompaliFiyati;

            PlayerPrefs.SetInt("ToplamPara", toplamPara);
            PlayerPrefs.SetInt("Pompali_SatinAlindi", 1);
            PlayerPrefs.Save();

            if (magazaPompaliVerisi != null)
            {
                magazaPompaliVerisi.satinAlindi = true;
                magazaPompaliVerisi.ButonUIGuncelle();
            }

            ParaUIGuncelle();
            // 🎯 BAŞARILI BİLDİRİMİ
            BildirimGoster("POMPALI BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
            // ❌ YETERSİZ BAKİYE BİLDİRİMİ
            BildirimGoster("YETERSİZ BAKİYE! ZOMBİ AVLA.", Color.red);
        }
    }

    void ParaUIGuncelle()
    {
        if (magazaParaYazisi != null)
            magazaParaYazisi.text = toplamPara.ToString();
    }

    // 🌟 2 SANİYELİĞE EKRANDA YAZI ÇIKARTAN SİHİRLİ FONKSİYON kanka
    public void BildirimGoster(string mesaj, Color yaziRengi)
    {
        if (bildirimYazisiText != null)
        {
            StopAllCoroutines(); // Eğer üst üste basarsa eski zamanlayıcıyı sıfırlar
            StartCoroutine(BildirimZamanlayici(mesaj, yaziRengi));
        }
    }

    private IEnumerator BildirimZamanlayici(string mesaj, Color yaziRengi)
    {
        bildirimYazisiText.text = mesaj;
        bildirimYazisiText.color = yaziRengi;
        bildirimYazisiText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f); // Ekranda 2 saniye kalır kanka

        bildirimYazisiText.gameObject.SetActive(false);
    }
}