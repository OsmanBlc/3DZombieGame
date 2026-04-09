using UnityEngine;

public class SilahAtes : MonoBehaviour
{
    [Header("Silah İstatistikleri")]
    public float hasar = 25f;           // Zombiye verilecek hasar
    public float menzil = 100f;         // Merminin gidebileceği maksimum mesafe
    public float atisAraligi = 0.5f;    // İki mermi arasındaki bekleme süresi (Saniye)

    private float birSonrakiAtisZamani = 0f; // Kodun atış zamanını takip etmesi için

    [Header("Referanslar")]
    public Camera oyuncuKamerasi;       // Nişan alınan kamera
    public ParticleSystem namluAtesi;   // Muzzle flash efekti
    public AudioSource silahSesi;       // Silahın patlama sesi

    void Update()
    {
        // Sol tıka basıldıysa VE ateş etmek için gereken süre geçtiyse
        if (Input.GetMouseButtonDown(0) && Time.time >= birSonrakiAtisZamani)
        {
            // Bir sonraki atış zamanını ayarla (Şu anki zaman + bekleme süresi)
            birSonrakiAtisZamani = Time.time + atisAraligi;

            AtesEt();
        }
    }

    void AtesEt()
    {
        // 1. GÖRSEL EFEKT: Muzzle Flash oynat
        if (namluAtesi != null)
        {
            namluAtesi.Stop(); // Eğer önceki patlama hala ekrandaysa onu anında kes
            namluAtesi.Play(); // Efekti baştan ve temiz bir şekilde oynat
        }

        // 2. SES EFEKTİ: Silah sesini çal
        if (silahSesi != null)
        {
            silahSesi.Play();
        }

        // 3. MERMİ MANTIĞI: Raycast (Işın) gönder
        // DEBUG: Merminin nereye gittiğini görmek için Scene ekranında 2 saniye kalacak kırmızı bir lazer çizer
        Debug.DrawRay(oyuncuKamerasi.transform.position, oyuncuKamerasi.transform.forward * menzil, Color.red, 2f);

        // MERMİ MANTIĞI: Raycast gönder
        RaycastHit hit;
        if (Physics.Raycast(oyuncuKamerasi.transform.position, oyuncuKamerasi.transform.forward, out hit, menzil))
        {
            // İlk olarak ışının tam olarak NEYE çarptığını konsola yazdıralım
            Debug.Log("Işın şuna çarptı: " + hit.transform.name);

            // ZombiCan kontrolü
            ZombiCan vurulanZombi = hit.transform.GetComponent<ZombiCan>();
            if (vurulanZombi != null)
            {
                vurulanZombi.HasarAl(hasar);
            }
        }
        else
        {
            Debug.Log("Mermi boşluğa gitti!");
        }
    }
}