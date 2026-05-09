using UnityEngine;

public class CanPaketi : MonoBehaviour
{
    [Header("Can Paketi Ayarlari")]
    public float iyilestirmeMiktari = 30f;

    [Header("Ses")]
    public AudioClip almaSesi;
    public float sesSesviyesi = 0.9f;

    [Header("Animasyon")]
    public float donmeHizi = 80f;
    public float sallanmaHizi = 1.5f;
    public float sallanmaYuksekligi = 0.2f;

    private Vector3 baslangicPozisyonu;

    void Start()
    {
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, donmeHizi * Time.deltaTime, Space.World);
        float yOffset = Mathf.Sin(Time.time * sallanmaHizi) * sallanmaYuksekligi;
        transform.position = baslangicPozisyonu + new Vector3(0f, yOffset, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.Heal(iyilestirmeMiktari);

        if (almaSesi != null)
            AudioSource.PlayClipAtPoint(almaSesi, transform.position, sesSesviyesi * SettingsManager.SfxVolume);

        Destroy(gameObject);
    }
}
