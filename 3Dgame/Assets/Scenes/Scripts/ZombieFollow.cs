using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            if (direction.magnitude > 0.1f)
            {
                transform.position += direction.normalized * speed * Time.deltaTime;
            }
        }
    }
}