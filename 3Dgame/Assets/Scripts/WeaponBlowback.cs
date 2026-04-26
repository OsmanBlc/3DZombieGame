using UnityEngine;

public class WeaponBlowback : MonoBehaviour
{
    [Header("Parçalar")]
    public Transform slideTransform;

    [Header("Ayarlar")]
    public float blowbackDistance = 0.1f; // 1 yerine 0.1 gibi küçük bir değerle başlayalım
    public float returnSpeed = 15f;

    [Tooltip("Kırmızı ok ileri bakıyorsa geriye gitmek için X'i -1 yapın")]
    public Vector3 blowbackDirection = new Vector3(-1, 0, 0);

    private Vector3 originalPosition;

    void Start()
    {
        if (slideTransform != null)
        {
            originalPosition = slideTransform.localPosition;
        }
    }

    void Update()
    {
        if (slideTransform == null) return;
        // Kızağı her karede kendi orijinal yerine doğru pürüzsüzce çek
        slideTransform.localPosition = Vector3.Lerp(slideTransform.localPosition, originalPosition, Time.deltaTime * returnSpeed);
    }

    public void ApplyBlowback()
    {
        if (slideTransform == null) return;

        Debug.Log("Geri tepme kodu çalıştı!"); // Konsola bilgi gönderiyoruz

        // Kızağı belirtilen yöne ve mesafeye anında fırlat
        slideTransform.localPosition = originalPosition + (blowbackDirection.normalized * blowbackDistance);
    }
}