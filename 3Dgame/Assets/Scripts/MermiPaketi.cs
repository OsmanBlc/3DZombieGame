using UnityEngine;

public class MermiPaketi : MonoBehaviour
{
    [Header("Mermi Paketi Ayarlari")]
    public int mermiMiktari = 30;

    [Header("Ses")]
    public AudioClip almaSesi;
    public float sesSesviyesi = 0.9f;

    [Header("Animasyon")]
    public float donmeHizi = 90f;
    public float sallanmaHizi = 1.8f;
    public float sallanmaYuksekligi = 0.18f;

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

        SilahAtes[] silahlar = FindObjectsByType<SilahAtes>(FindObjectsSortMode.None);
        foreach (SilahAtes silah in silahlar)
            silah.YedekMermiEkle(mermiMiktari);

        if (almaSesi != null)
            AudioSource.PlayClipAtPoint(almaSesi, transform.position, sesSesviyesi * SettingsManager.SfxVolume);

        Destroy(gameObject);
    }
}
