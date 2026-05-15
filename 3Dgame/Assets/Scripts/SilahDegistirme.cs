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

    void Start()
    {
        // BUG ENGELLEME: Oyun ilk açıldığında aktifSilahIndex dışındaki 
        // tüm silahları kodla kesin olarak kapatıyoruz.
        if (silahlar != null && silahlar.Length > 0)
        {
            for (int i = 0; i < silahlar.Length; i++)
            {
                if (silahlar[i] != null)
                    silahlar[i].SetActive(i == aktifSilahIndex);
            }
        }

        SilahIkonunuGuncelle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SilahSec(0);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SilahSec(1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SilahSec(2);
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            SilahSec(aktifSilahIndex + 1);
        }
        else if (scroll < 0f)
        {
            SilahSec(aktifSilahIndex - 1);
        }
    }

    void SilahSec(int yeniSilahIndex)
    {
        if (silahlar == null || silahlar.Length == 0)
            return;

        if (yeniSilahIndex >= silahlar.Length)
            yeniSilahIndex = 0;
        else if (yeniSilahIndex < 0)
            yeniSilahIndex = silahlar.Length - 1;

        // Zaten o silah seçiliyse tekrar işlem yapma
        if (aktifSilahIndex == yeniSilahIndex && silahlar[aktifSilahIndex].activeSelf)
            return;

        aktifSilahIndex = yeniSilahIndex;
        SilahlariGuncelle();
    }

    void SilahlariGuncelle()
    {
        if (silahlar == null || silahlar.Length == 0)
            return;

        // Aktif olan klasörü açar, diğer iki klasörü kapatır
        for (int i = 0; i < silahlar.Length; i++)
        {
            if (silahlar[i] != null)
                silahlar[i].SetActive(i == aktifSilahIndex);
        }

        SilahIkonunuGuncelle();
    }

    void SilahIkonunuGuncelle()
    {
        if (silahIkonu != null && silahSprite != null && silahSprite.Length > aktifSilahIndex)
        {
            silahIkonu.sprite = silahSprite[aktifSilahIndex];

            // Tabanca (Element 0) normal yön ve normal boyut
            if (aktifSilahIndex == 0)
            {
                silahIkonu.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
            // SMG (Element 1) ters yön ve daha küçük
            else if (aktifSilahIndex == 1)
            {
                silahIkonu.rectTransform.localScale = new Vector3(-0.65f, 0.65f, 1f);
            }
            // Shotgun (Element 2) ters yön ve daha küçük
            else if (aktifSilahIndex == 2)
            {
                silahIkonu.rectTransform.localScale = new Vector3(-0.60f, 0.60f, 1f);
            }
        }
    }
}