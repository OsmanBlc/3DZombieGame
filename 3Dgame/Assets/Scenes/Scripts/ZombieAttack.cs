using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public float damagePerSecond = 5f;

    private PlayerHealth playerHealth;
    private bool isAttacking = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            isAttacking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            playerHealth = null;
        }
    }

    void Update()
    {
        if (isAttacking && playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}