using UnityEngine;

public class CarEscape : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Alana giren nesne oyuncu mu diye bakýyoruz
        if (other.CompareTag("Player"))
        {
            // BolumManager'a ulaþýp araba görevini tamamla diyoruz
            if (BolumManager.Instance != null)
            {
                BolumManager.Instance.ArabayaUlasildi();
            }
        }
    }
}