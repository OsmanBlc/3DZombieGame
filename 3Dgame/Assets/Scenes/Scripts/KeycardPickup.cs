using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Level1Manager.hasKeycard = true;
            Debug.Log("Keycard collected!");
            Destroy(gameObject);
        }
    }
}