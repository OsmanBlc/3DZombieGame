using UnityEngine;
using TMPro; // TextMeshPro kullanacağımız için bu kütüphane şart kanka

public class AnaMenuParaGosterici : MonoBehaviour
{
    [Header("UI Bağlantısı")]
    [SerializeField] private TMP_Text anaMenuParaText; // Paranın yazılacağı TMP nesnesi

    void Start()
    {
        // 🎯 HAFIZADAN OKU: Oyun sahnesinde zombilerden kazanıp kaydettiğimiz "ToplamPara"yı çekiyoruz.
        // Eğer daha önce hiç para kazanılmadıysa varsayılan olarak 0 getirir kanka.
        int toplamPara = PlayerPrefs.GetInt("ToplamPara", 0);

        // UI üzerindeki yazıyı güncelle
        if (anaMenuParaText != null)
        {
            anaMenuParaText.text = toplamPara.ToString();
        }
        else
        {
            Debug.LogWarning("Kanka, AnaMenuParaGosterici scriptine bir TextMeshPro objesi bağlamadın!");
        }
    }
}