using System.Collections.Generic;
using UnityEngine;

public class PaketSpawner : MonoBehaviour
{
    [Header("Paket Prefablari")]
    public GameObject canPaketiPrefab;
    public GameObject mermiPaketiPrefab;

    [Header("Can Paketi Noktalari")]
    public Transform canPaketiNoktalariParent;
    public Transform[] canPaketiNoktalari;

    [Header("Mermi Paketi Noktalari")]
    public Transform mermiPaketiNoktalariParent;
    public Transform[] mermiPaketiNoktalari;

    [Header("Spawn Ayarlari")]
    public bool noktaRotasyonunuKullan = true;
    public float spawnYukseklikOffset = 0f;

    void Start()
    {
        SpawnPaketler(canPaketiPrefab, SpawnNoktalariGetir(canPaketiNoktalari, canPaketiNoktalariParent), "canPaketiPrefab");
        SpawnPaketler(mermiPaketiPrefab, SpawnNoktalariGetir(mermiPaketiNoktalari, mermiPaketiNoktalariParent), "mermiPaketiPrefab");
    }

    Transform[] SpawnNoktalariGetir(Transform[] spawnNoktalari, Transform parent)
    {
        if (spawnNoktalari != null && spawnNoktalari.Length > 0)
            return spawnNoktalari;

        if (parent == null)
            return new Transform[0];

        List<Transform> noktalar = new List<Transform>();
        foreach (Transform child in parent)
            noktalar.Add(child);

        return noktalar.ToArray();
    }

    void SpawnPaketler(GameObject prefab, Transform[] spawnNoktalari, string prefabAdi)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"PaketSpawner: {prefabAdi} atanmamis!");
            return;
        }

        if (spawnNoktalari == null || spawnNoktalari.Length == 0)
            return;

        foreach (Transform spawnNoktasi in spawnNoktalari)
        {
            if (spawnNoktasi == null)
                continue;

            Vector3 pozisyon = spawnNoktasi.position + Vector3.up * spawnYukseklikOffset;
            Quaternion rotasyon = noktaRotasyonunuKullan ? spawnNoktasi.rotation : Quaternion.identity;
            Instantiate(prefab, pozisyon, rotasyon);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.8f);
        CizSpawnNoktalari(SpawnNoktalariGetir(canPaketiNoktalari, canPaketiNoktalariParent), 0.35f);

        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.8f);
        CizSpawnNoktalari(SpawnNoktalariGetir(mermiPaketiNoktalari, mermiPaketiNoktalariParent), 0.35f);
    }

    void CizSpawnNoktalari(Transform[] spawnNoktalari, float yaricap)
    {
        if (spawnNoktalari == null)
            return;

        foreach (Transform spawnNoktasi in spawnNoktalari)
        {
            if (spawnNoktasi == null)
                continue;

            Gizmos.DrawWireSphere(spawnNoktasi.position, yaricap);
        }
    }
}
