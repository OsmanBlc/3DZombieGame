using UnityEngine;

public class PaketSpawner : MonoBehaviour
{
    [Header("Paket Prefablari")]
    public GameObject canPaketiPrefab;
    public GameObject mermiPaketiPrefab;

    [Header("Sayi Ayarlari")]
    public int canPaketiSayisi = 4;
    public int mermiPaketiSayisi = 6;

    [Header("Belirli Noktalar (Opsiyonel)")]
    public Transform[] belirliNoktalari;

    [Header("Rastgele Yayilim")]
    public bool rastgeleYayilim = true;
    public float spawnYaricapi = 40f;
    public LayerMask zeminKatmani = ~0;
    public float raycastYuksekligi = 50f;
    public float spawnYerdenYukseklik = 0.5f;

    void Start()
    {
        int toplamPaket = canPaketiSayisi + mermiPaketiSayisi;

        if (belirliNoktalari != null && belirliNoktalari.Length > 0)
        {
            SpawnBelirliNoktalarda(toplamPaket);
        }
        else if (rastgeleYayilim)
        {
            SpawnRastgele();
        }
    }

    void SpawnBelirliNoktalarda(int toplamPaket)
    {
        Transform[] karistirilan = (Transform[])belirliNoktalari.Clone();
        FisherYatesShuffle(karistirilan);

        int index = 0;
        int toplamNokta = karistirilan.Length;

        for (int i = 0; i < canPaketiSayisi && index < toplamNokta; i++, index++)
        {
            if (canPaketiPrefab != null)
                Instantiate(canPaketiPrefab, karistirilan[index].position, Quaternion.identity);
        }

        for (int i = 0; i < mermiPaketiSayisi && index < toplamNokta; i++, index++)
        {
            if (mermiPaketiPrefab != null)
                Instantiate(mermiPaketiPrefab, karistirilan[index].position, Quaternion.identity);
        }
    }

    void SpawnRastgele()
    {
        for (int i = 0; i < canPaketiSayisi; i++)
        {
            Vector3? nokta = RastgeleZeminNoktasi();
            if (nokta.HasValue && canPaketiPrefab != null)
                Instantiate(canPaketiPrefab, nokta.Value, Quaternion.identity);
        }

        for (int i = 0; i < mermiPaketiSayisi; i++)
        {
            Vector3? nokta = RastgeleZeminNoktasi();
            if (nokta.HasValue && mermiPaketiPrefab != null)
                Instantiate(mermiPaketiPrefab, nokta.Value, Quaternion.identity);
        }
    }

    Vector3? RastgeleZeminNoktasi()
    {
        for (int deneme = 0; deneme < 15; deneme++)
        {
            Vector2 daire = Random.insideUnitCircle * spawnYaricapi;
            Vector3 baslangic = transform.position + new Vector3(daire.x, raycastYuksekligi, daire.y);

            if (Physics.Raycast(baslangic, Vector3.down, out RaycastHit hit, raycastYuksekligi * 2f, zeminKatmani))
            {
                return hit.point + Vector3.up * spawnYerdenYukseklik;
            }
        }

        return null;
    }

    void FisherYatesShuffle(Transform[] dizi)
    {
        for (int i = dizi.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (dizi[i], dizi[j]) = (dizi[j], dizi[i]);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnYaricapi);
    }
}
