using UnityEngine;

public class ManualZombieSpawner : MonoBehaviour
{
    [Header("Zombie Ayarlari")]
    public GameObject zombiPrefab;

    [Header("Spawn Ayarlari")]
    public bool noktaRotasyonunuKullan = true;
    public float spawnYukseklikOffset = 0f;

    void Start()
    {
        if (zombiPrefab == null)
        {
            Debug.LogWarning("ManualZombieSpawner: zombiPrefab atanmamis!");
            return;
        }

        if (transform.childCount == 0)
        {
            Debug.LogWarning("ManualZombieSpawner: ZombieSpawner altinda spawn noktasi yok!");
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Transform playerTransform = playerObj != null ? playerObj.transform : null;

        Debug.Log($"ManualZombieSpawner: {transform.childCount} noktaya zombi spawn edilecek.");

        foreach (Transform spawnNoktasi in transform)
        {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.8f);

        foreach (Transform spawnNoktasi in transform)
            Gizmos.DrawWireSphere(spawnNoktasi.position, 0.5f);
    }
}
