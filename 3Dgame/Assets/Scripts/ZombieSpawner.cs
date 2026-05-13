using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Ayarlari")]
    public GameObject zombiPrefab;

    [Header("Spawn Noktalari")]
    public Transform[] zombiSpawnNoktalari;
    public bool noktaRotasyonunuKullan = true;
    public float spawnYukseklikOffset = 0f;

    void Start()
    {
        if (zombiPrefab == null)
        {
            Debug.LogWarning("ZombieSpawner: zombiPrefab atanmamis!");
            return;
        }

        Transform[] spawnNoktalari = SpawnNoktalariGetir();
        if (spawnNoktalari.Length == 0)
        {
            Debug.LogWarning("ZombieSpawner: Spawn noktasi yok. ZombieSpawner altina ZombiePoint objeleri ekle veya zombiSpawnNoktalari listesine nokta surukle.");
            return;
        }

        Debug.Log($"ZombieSpawner: {spawnNoktalari.Length} noktaya zombi spawn edilecek.");

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Transform playerTransform = playerObj != null ? playerObj.transform : null;

        foreach (Transform spawnNoktasi in spawnNoktalari)
        {
            if (spawnNoktasi == null)
                continue;

            Vector3 pozisyon = spawnNoktasi.position + Vector3.up * spawnYukseklikOffset;
            Quaternion rotasyon = noktaRotasyonunuKullan ? spawnNoktasi.rotation : Quaternion.identity;
            GameObject zombi = Instantiate(zombiPrefab, pozisyon, rotasyon);

            if (playerTransform == null)
                continue;

            ZombieFollow follow = zombi.GetComponent<ZombieFollow>();
            if (follow != null)
                follow.player = playerTransform;
        }
    }

    Transform[] SpawnNoktalariGetir()
    {
        if (zombiSpawnNoktalari != null && zombiSpawnNoktalari.Length > 0)
            return zombiSpawnNoktalari;

        List<Transform> noktalar = new List<Transform>();
        foreach (Transform child in transform)
            noktalar.Add(child);

        return noktalar.ToArray();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.8f);
        foreach (Transform spawnNoktasi in SpawnNoktalariGetir())
        {
            if (spawnNoktasi == null)
                continue;

            Gizmos.DrawWireSphere(spawnNoktasi.position, 0.5f);
        }
    }
}
