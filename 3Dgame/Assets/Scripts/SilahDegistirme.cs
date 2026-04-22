using UnityEngine;
using UnityEngine.UI;

public class SilahDegistirici : MonoBehaviour
{
    [Header("Silahlar")]
    public GameObject[] silahlar;
    private int aktifSilahIndex = 0;

    [Header("UI")]
    public Image silahIkonu;
    public Sprite[] silahSprite;

    void Start()
    {
        SilahlariGuncelle();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            aktifSilahIndex++;

            if (aktifSilahIndex >= silahlar.Length)
                aktifSilahIndex = 0;

            SilahlariGuncelle();
        }
        else if (scroll < 0f)
        {
            aktifSilahIndex--;

            if (aktifSilahIndex < 0)
                aktifSilahIndex = silahlar.Length - 1;

            SilahlariGuncelle();
        }
    }

    void SilahlariGuncelle()
    {
        if (silahlar == null || silahlar.Length == 0)
            return;

        // Aktif silahı aç, diğerlerini kapat
        for (int i = 0; i < silahlar.Length; i++)
        {
            if (silahlar[i] != null)
                silahlar[i].SetActive(i == aktifSilahIndex);
        }

        // UI ikonunu değiştir
        if (silahIkonu != null && silahSprite != null && silahSprite.Length > aktifSilahIndex)
        {
            silahIkonu.sprite = silahSprite[aktifSilahIndex];

            // Tabanca normal yön ve normal boyut
            if (aktifSilahIndex == 0)
            {
                silahIkonu.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
            // SMG ters yön ve daha küçük
            else if (aktifSilahIndex == 1)
            {
                silahIkonu.rectTransform.localScale = new Vector3(-0.65f, 0.65f, 1f);
            }
            // Shotgun ters yön ve daha küçük
            else if (aktifSilahIndex == 2)
            {
                silahIkonu.rectTransform.localScale = new Vector3(-0.60f, 0.60f, 1f);
            }
        }
    }
}