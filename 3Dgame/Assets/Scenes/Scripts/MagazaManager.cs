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

    [Header("Silah / Eşya Fiyatları")]
    public int smgFiyati = 1500;
    public int pompaliFiyati = 3000;
    public int canPaketiFiyati = 500;
    public int mermiFiyati = 300;
    public int elBombasiFiyati = 750;

    [Header("Silah / Eşya Veri Bağlantıları")]
    public SilahVerisi magazaSmgVerisi;
    public SilahVerisi magazaPompaliVerisi;
    public SilahVerisi magazaCanPaketiVerisi;
    public SilahVerisi magazaMermiVerisi;
    public SilahVerisi magazaElBombasiVerisi;

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
            BildirimGoster("SMG BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
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
            BildirimGoster("POMPALI BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
            BildirimGoster("YETERSİZ BAKİYE! ZOMBİ AVLA.", Color.red);
        }
    }

    // 🔴 CAN PAKETİ SATIN ALMA (TEK SEFERLİK)
    public void CanPaketiSatinAl()
    {
        if (PlayerPrefs.GetInt("Can Paketi_SatinAlindi", 0) == 1)
        {
            BildirimGoster("CAN PAKETİ KİLİDİ ZATEN AÇIK!", Color.yellow);
            return;
        }

        if (toplamPara >= canPaketiFiyati)
        {
            toplamPara -= canPaketiFiyati;

            PlayerPrefs.SetInt("ToplamPara", toplamPara);
            PlayerPrefs.SetInt("Can Paketi_SatinAlindi", 1);
            PlayerPrefs.Save();

            if (magazaCanPaketiVerisi != null)
            {
                magazaCanPaketiVerisi.satinAlindi = true;
                magazaCanPaketiVerisi.ButonUIGuncelle();
            }

            ParaUIGuncelle();
            BildirimGoster("CAN PAKETİ BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
            BildirimGoster("YETERSİZ BAKİYE!", Color.red);
        }
    }

    // 🟢 MERMİ SATIN ALMA (TEK SEFERLİK)
    public void MermiSatinAl()
    {
        if (PlayerPrefs.GetInt("Mermi_SatinAlindi", 0) == 1)
        {
            BildirimGoster("MERMİ KİLİDİ ZATEN AÇIK!", Color.yellow);
            return;
        }

        if (toplamPara >= mermiFiyati)
        {
            toplamPara -= mermiFiyati;

            PlayerPrefs.SetInt("ToplamPara", toplamPara);
            PlayerPrefs.SetInt("Mermi_SatinAlindi", 1);
            PlayerPrefs.Save();

            if (magazaMermiVerisi != null)
            {
                magazaMermiVerisi.satinAlindi = true;
                magazaMermiVerisi.ButonUIGuncelle();
            }

            ParaUIGuncelle();
            BildirimGoster("MERMİ BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
            BildirimGoster("YETERSİZ BAKİYE!", Color.red);
        }
    }

    // 🔵 EL BOMBASI SATIN ALMA (TEK SEFERLİK)
    public void ElBombasiSatinAl()
    {
        if (PlayerPrefs.GetInt("El Bombasi_SatinAlindi", 0) == 1)
        {
            BildirimGoster("EL BOMBASI KİLİDİ ZATEN AÇIK!", Color.yellow);
            return;
        }

        if (toplamPara >= elBombasiFiyati)
        {
            toplamPara -= elBombasiFiyati;

            PlayerPrefs.SetInt("ToplamPara", toplamPara);
            PlayerPrefs.SetInt("El Bombasi_SatinAlindi", 1);
            PlayerPrefs.Save();

            if (magazaElBombasiVerisi != null)
            {
                magazaElBombasiVerisi.satinAlindi = true;
                magazaElBombasiVerisi.ButonUIGuncelle();
            }

            ParaUIGuncelle();
            BildirimGoster("EL BOMBASI BAŞARIYLA SATIN ALINDI!", Color.green);
        }
        else
        {
            BildirimGoster("YETERSİZ BAKİYE!", Color.red);
        }
    }

    void ParaUIGuncelle()
    {
        if (magazaParaYazisi != null)
            magazaParaYazisi.text = toplamPara.ToString();
    }

    public void BildirimGoster(string mesaj, Color yaziRengi)
    {
        if (bildirimYazisiText != null)
        {
            StopAllCoroutines();
            StartCoroutine(BildirimZamanlayici(mesaj, yaziRengi));
        }
    }

    private IEnumerator BildirimZamanlayici(string mesaj, Color yaziRengi)
    {
        bildirimYazisiText.text = mesaj;
        bildirimYazisiText.color = yaziRengi;
        bildirimYazisiText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        bildirimYazisiText.gameObject.SetActive(false);
    }
}