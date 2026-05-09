using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Ayarlari")]
    public GameObject zombiPrefab;
    public int zombiSayisi = 8;

    [Header("Spawn Alani")]
    public float spawnYaricapi = 40f;
    public LayerMask zeminKatmani = ~0;
    public float raycastYuksekligi = 50f;
    public float spawnYerdenYukseklik = 0.5f;

    void Start()
    {
        if (zombiPrefab == null)
        {
            Debug.LogWarning("ZombieSpawner: zombiPrefab atanmamış!");
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Transform playerTransform = playerObj != null ? playerObj.transform : null;

        for (int i = 0; i < zombiSayisi; i++)
        {
            Vector3? nokta = RastgeleZeminNoktasi();
            if (!nokta.HasValue)
                continue;

            GameObject zombi = Instantiate(zombiPrefab, nokta.Value, Quaternion.identity);

            if (playerTransform != null)
            {
                ZombieFollow follow = zombi.GetComponent<ZombieFollow>();
                if (follow != null)
                    follow.player = playerTransform;
            }
        }
    }

    Vector3? RastgeleZeminNoktasi()
    {
        for (int deneme = 0; deneme < 15; deneme++)
        {
            Vector2 daire = Random.insideUnitCircle * spawnYaricapi;
            Vector3 baslangic = transform.position + new Vector3(daire.x, raycastYuksekligi, daire.y);

            if (Physics.Raycast(baslangic, Vector3.down, out RaycastHit hit, raycastYuksekligi * 2f, zeminKatmani))
                return hit.point + Vector3.up * spawnYerdenYukseklik;
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnYaricapi);
    }
}
