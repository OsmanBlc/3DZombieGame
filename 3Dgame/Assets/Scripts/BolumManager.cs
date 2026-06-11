using System.Collections;
using UnityEngine;

public class BolumManager : MonoBehaviour
{
    public static BolumManager Instance { get; private set; }

    [Header("Yıldız Süre Limitleri (saniye)")]
    public float ucYildizSure = 60f;
    public float ikiYildizSure = 120f;

    private int toplamZombi;
    private int oldurulenZombi;
    private float baslangicZamani;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        baslangicZamani = Time.time;
        oldurulenZombi = 0;
        StartCoroutine(ZombiSay());
    }

    IEnumerator ZombiSay()
    {
        yield return null;
        ZombiCan[] zombiler = FindObjectsByType<ZombiCan>(FindObjectsSortMode.None);
        toplamZombi = zombiler.Length;
    }

    // 🧰 Zombi öldüğünde sadece sayacı artırıyoruz, oyunu bitirmiyoruz.
    public static void ZombieOlduruldu()
    {
        if (Instance == null)
            return;

        Instance.oldurulenZombi++;

        // NOT: Eskiden tüm zombiler ölünce GameFlowManager tetikleniyordu, 
        // artık oyuncunun arabaya gitmesi gerektiği için o bitiş şartını buradan kaldırdık.
    }

    // 🚗 Arabaya ulaşıldığında bu fonksiyon dışarıdan çağrılacak ve bölümü bitirecek.
    public void ArabayaUlasildi()
    {
        float gecenSure = Time.time - baslangicZamani;

        // Senin orijinal yıldız ve ekran gösterme kodun:
        GameFlowManager.ShowLevelComplete(gecenSure, ucYildizSure, ikiYildizSure);
    }
}