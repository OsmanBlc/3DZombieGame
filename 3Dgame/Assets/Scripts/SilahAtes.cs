using UnityEngine;
using UnityEngine.UI; // YENİ: UI elementlerini (Image) kodla değiştirebilmek için bu kütüphane şart!

public class SilahAtes : MonoBehaviour
{
    [Header("Silah İstatistikleri")]
    public float hasar = 25f;
    public float menzil = 100f;
    public float atisAraligi = 0.5f;
    private float birSonrakiAtisZamani = 0f;

    [Header("Referanslar")]
    public Camera oyuncuKamerasi;
    public ParticleSystem namluAtesi;
    public AudioSource silahSesi;

    [Header("Nişangah Ayarları")]
    public Image nisangahGorseli;            // UI'daki Crosshair görselimiz
    public Color normalRenk = Color.white;   // Duvara/boşluğa bakarkenki renk
    public Color zombiRengi = Color.red;     // Zombiye bakarkenki renk

    void Update()
    {
        // 1. Sürekli olarak nereye baktığımızı kontrol et (Nişangah rengi için)
        NisangahKontrol();

        // 2. Ateş etme kontrolü
        if (Input.GetMouseButtonDown(0) && Time.time >= birSonrakiAtisZamani)
        {
            birSonrakiAtisZamani = Time.time + atisAraligi;
            AtesEt();
        }
    }

    void NisangahKontrol()
    {
        // Nişangah atanmamışsa oyun çökmesin diye güvenlik önlemi
        if (nisangahGorseli == null) return;

        RaycastHit hit;
        // Kameradan ileriye doğru sürekli bir radar ışını yolluyoruz
        if (Physics.Raycast(oyuncuKamerasi.transform.position, oyuncuKamerasi.transform.forward, out hit, menzil))
        {
            // Baktığımız şeyin üzerinde 'ZombiCan' kodu var mı?
            ZombiCan bakilanZombi = hit.transform.GetComponent<ZombiCan>();

            if (bakilanZombi != null)
            {
                // Zombiye bakıyoruz! Rengi kırmızı yap.
                nisangahGorseli.color = zombiRengi;
            }
            else
            {
                // Zombi dışında bir şeye (duvar, yol) bakıyoruz. Beyaz yap.
                nisangahGorseli.color = normalRenk;
            }
        }
        else
        {
            // Mermi menzilinde hiçbir şey yok (Gökyüzüne bakıyoruz). Beyaz yap.
            nisangahGorseli.color = normalRenk;
        }
    }

    void AtesEt()
    {
        // ... (Eski Ateş etme, Ses, Efekt ve Zombiye Hasar verme kodlarımız burada aynı şekilde duruyor)

        if (namluAtesi != null)
        {
            namluAtesi.Stop();
            namluAtesi.Play();
        }

        if (silahSesi != null)
        {
            silahSesi.Play();
        }

        RaycastHit hit;
        if (Physics.Raycast(oyuncuKamerasi.transform.position, oyuncuKamerasi.transform.forward, out hit, menzil))
        {
            ZombiCan vurulanZombi = hit.transform.GetComponent<ZombiCan>();
            if (vurulanZombi != null)
            {
                vurulanZombi.HasarAl(hasar);
            }
        }

        // Silah sekme kodu tetikleyicisi
        SilahHissiyat hissiyat = GetComponent<SilahHissiyat>();
        if (hissiyat != null)
        {
            hissiyat.GeriTepmeUygula();
        }
    }
}