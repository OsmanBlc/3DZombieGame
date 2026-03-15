using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform player;
    public float spawnRadius = 15f;
    public float spawnTime = 3f;

    void Start()
    {
        InvokeRepeating("SpawnZombie", 2f, spawnTime);
    }

    void SpawnZombie()
    {
        float x = Random.Range(-spawnRadius, spawnRadius);
        float z = Random.Range(-spawnRadius, spawnRadius);

        Vector3 spawnPosition = new Vector3(x, 1, z);

        GameObject newZombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

        ZombieFollow zombieFollow = newZombie.GetComponent<ZombieFollow>();
        if (zombieFollow != null)
        {
            zombieFollow.player = player;
        }
    }
}