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

    public static void ZombieOlduruldu()
    {
        if (Instance == null)
            return;

        Instance.oldurulenZombi++;

        if (Instance.toplamZombi > 0 && Instance.oldurulenZombi >= Instance.toplamZombi)
        {
            float gecenSure = Time.time - Instance.baslangicZamani;
            GameFlowManager.ShowLevelComplete(gecenSure, Instance.ucYildizSure, Instance.ikiYildizSure);
        }
    }
}
