using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SilahAtes : MonoBehaviour
{
    public enum SilahTuru { Tekli, Otomatik, Pompali }

    [Header("Silah İstatistikleri")]
    public SilahTuru silahTuru = SilahTuru.Tekli;
    public float hasar = 25f;
    public float kafaHasarCarpani = 2f;
    public float menzil = 100f;
    public float atisAraligi = 0.5f;
    public WeaponBlowback blowbackScript;
    private float birSonrakiAtisZamani = 0f;

    [Header("Pompalı Ayarları (Sadece Pompali seçiliyse)")]
    public int pompaliMermiSayisi = 8;
    public float pompaliSacilmaAcisi = 0.05f;

    [Header("Mermi Sistemi")]
    public int sarjorKapasitesi = 15;
    public int mevcutMermi;
    public int yedekMermi = 60;
    public float reloadSuresi = 2f;
    public AudioClip reloadSesi;
    public float reloadSesSeviyesi = 0.8f;
    public AudioClip mermiYokSesi;
    public float mermiYokSesSeviyesi = 0.8f;
    public float mermiYokSesAraligi = 0.25f;
    private bool yenidenDolduruyor = false;

    [Header("Referanslar")]
    public Camera oyuncuKamerasi;
    public ParticleSystem namluAtesi;
    public AudioSource silahSesi;

    [Header("UI")]
    public TMP_Text mermiYazisi;
    public Color normalMermiRengi = Color.white;
    public Color azMermiRengi = new Color(1f, 0.78f, 0.12f);
    public Color kritikMermiRengi = new Color(1f, 0.12f, 0.08f);
    public int azMermiSiniri = 5;
    public int kritikMermiSiniri = 2;

    [Header("Nişangah Ayarları")]
    public Image nisangahGorseli;
    public Color normalRenk = Color.white;
    public Color zombiRengi = Color.red;
    public Color vurusRengi = new Color(1f, 0.95f, 0.25f);
    public Color headshotRengi = new Color(1f, 0.15f, 0.08f);
    public float vurusBuyumeMiktari = 1.65f;
    public float headshotBuyumeCarpani = 1.25f;
    public float vurusEfektSuresi = 0.12f;
    public AudioClip vurusSesi;
    public AudioClip headshotSesi;
    public float vurusSesSeviyesi = 0.65f;

    [Header("Efektler")]
    public GameObject kanEfekti;

    [Header("Mermi Objesi (Projectile) Ayarları")]
    [Tooltip("Namludan fırlayacak olan mermi/iz prefab'ını sürükle")]
    public GameObject mermiPrefab;
    [Tooltip("Merminin tam olarak nereden fırlayacağını seç (Namlu ucu)")]
    public Transform namluUcu;
    [Tooltip("Merminin uçuş hızı (Gerçekçi olması için 150-250 arası idealdir kanka)")]
    public float mermiHizi = 200f;
    [Tooltip("Mermi objesinin boşa gittiğinde kaç saniye sonra silineceği")]
    public float mermiImhaSuresi = 0.5f;

    private SilahHissiyat hissiyat;
    private Vector3 nisangahOrijinalScale;
    private Coroutine nisangahEfektiCoroutine;
    private bool nisangahEfektiCalisiyor = false;
    private float sonrakiMermiYokSesiZamani = 0f;

    void Start()
    {
        if (oyuncuKamerasi == null)
            oyuncuKamerasi = Camera.main;

        if (silahSesi == null)
            silahSesi = GetComponent<AudioSource>();

        hissiyat = GetComponentInParent<SilahHissiyat>();

        if (mevcutMermi <= 0)
            mevcutMermi = sarjorKapasitesi;

        if (nisangahGorseli != null)
            nisangahOrijinalScale = nisangahGorseli.rectTransform.localScale;

        MermiUIGuncelle();
    }

    void OnEnable()
    {
        MermiUIGuncelle();
    }

    void Update()
    {
        NisangahKontrol();

        if (yenidenDolduruyor)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (mevcutMermi < sarjorKapasitesi && yedekMermi > 0)
            {
                StartCoroutine(ReloadYap());
            }
            return;
        }

        bool atesBasildi = false;

        if (silahTuru == SilahTuru.Otomatik)
        {
            atesBasildi = Input.GetMouseButton(0);
        }
        else
        {
            atesBasildi = Input.GetMouseButtonDown(0);
        }

        if (atesBasildi && Time.time >= birSonrakiAtisZamani)
        {
            if (mevcutMermi > 0)
            {
                birSonrakiAtisZamani = Time.time + atisAraligi;
                AtesEt();
            }
            else
            {
                Debug.Log("Mermi bitti! Reload yap.");
                MermiYokSesiCal();
            }
        }
    }

    void NisangahKontrol()
    {
        if (nisangahGorseli == null || oyuncuKamerasi == null || nisangahEfektiCalisiyor)
            return;

        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // 🛡️ Mermilerin nişangah rengini bozmasını engellemek için IgnoreRaycast katmanını hariç tutuyoruz
        int mask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray, out hit, menzil, mask))
        {
            ZombiCan bakilanZombi = hit.transform.GetComponentInParent<ZombiCan>();

            if (bakilanZombi != null)
                nisangahGorseli.color = zombiRengi;
            else
                nisangahGorseli.color = normalRenk;
        }
        else
        {
            nisangahGorseli.color = normalRenk;
        }
    }

    void AtesEt()
    {
        if (oyuncuKamerasi == null)
            return;

        mevcutMermi--;
        MermiUIGuncelle();

        if (namluAtesi != null)
            namluAtesi.Play();

        if (silahSesi != null)
        {
            silahSesi.Stop();
            silahSesi.Play();
        }

        if (blowbackScript != null)
            blowbackScript.ApplyBlowback();

        // 💥 DOĞRULTU HESAPLAMA MOTORU: Kameranın tam baktığı hedef noktayı buluyoruz kanka
        Vector3 hedefNokta;
        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        int mask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray, out hit, menzil, mask))
        {
            hedefNokta = hit.point;
        }
        else
        {
            hedefNokta = ray.GetPoint(menzil);
        }

        // Mermileri bu hedef noktaya odaklayarak fırlatması için motoru tetikliyoruz kanka
        MermiIziOlustur(hedefNokta);

        if (silahTuru == SilahTuru.Pompali)
        {
            for (int i = 0; i < pompaliMermiSayisi; i++)
            {
                MermiYolla(true);
            }
        }
        else
        {
            MermiYolla(false);
        }

        KovanFirlat();

        if (hissiyat != null)
        {
            hissiyat.GeriTepmeUygula(silahTuru == SilahTuru.Otomatik);
        }
    }

    void MermiIziOlustur(Vector3 hedefNokta)
    {
        if (mermiPrefab == null || namluUcu == null) return;

        // Namludan hedefe doğru giden temiz yön vektörü kanka
        Vector3 firlamaYonu = (hedefNokta - namluUcu.position).normalized;

        if (silahTuru == SilahTuru.Pompali)
        {
            for (int i = 0; i < pompaliMermiSayisi; i++)
            {
                Vector3 sacmaYonu = firlamaYonu;
                sacmaYonu.x += Random.Range(-pompaliSacilmaAcisi, pompaliSacilmaAcisi);
                sacmaYonu.y += Random.Range(-pompaliSacilmaAcisi, pompaliSacilmaAcisi);
                sacmaYonu.z += Random.Range(-pompaliSacilmaAcisi, pompaliSacilmaAcisi);

                MermiFirlat(sacmaYonu.normalized);
            }
        }
        else
        {
            MermiFirlat(firlamaYonu);
        }
    }

    void MermiFirlat(Vector3 normalizeYon)
    {
        // 🛡️ REZALET DOĞRULTU SAPMASINI ÖNLEYEN KİLİT SATIR: 
        // Mermiyi namlu ucunda doğurup, gideceği tam yöne doğru kafasını çeviriyoruz aga.
        GameObject go = Instantiate(mermiPrefab, namluUcu.position, Quaternion.LookRotation(normalizeYon));

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = normalizeYon * mermiHizi;
        }

        Destroy(go, mermiImhaSuresi);
    }

    void MermiYolla(bool sacilmaVar)
    {
        Vector3 rayYonu = oyuncuKamerasi.transform.forward;

        if (sacilmaVar)
        {
            rayYonu.x += Random.Range(-pompaliSacilmaAcisi, pompaliSacilmaAcisi);
            rayYonu.y += Random.Range(-pompaliSacilmaAcisi, pompaliSacilmaAcisi);
            rayYonu.z += Random.Range(-pompaliSacilmaAcisi, pompaliSacilmaAcisi);
        }

        Ray ray = new Ray(oyuncuKamerasi.transform.position, rayYonu);
        RaycastHit hit;

        // 🛡️ Mermilerin hasar ışınını yutmasını engellemek için IgnoreRaycast katmanını burada da filtreledik aga
        int mask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray, out hit, menzil, mask))
        {
            ZombiCan vurulanZombi = hit.transform.GetComponentInParent<ZombiCan>();

            if (vurulanZombi != null)
            {
                float uygulanacakHasar = hasar;
                bool kafaVurusu = hit.collider.CompareTag("Head");

                if (kafaVurusu)
                {
                    uygulanacakHasar *= kafaHasarCarpani;
                }

                vurulanZombi.HasarAl(uygulanacakHasar, hit.point);

                if (kanEfekti != null)
                {
                    Instantiate(kanEfekti, hit.point, Quaternion.LookRotation(hit.normal));
                }

                if (nisangahGorseli != null)
                {
                    HitMarkerGoster(kafaVurusu);
                }
            }
        }
    }

    IEnumerator ReloadYap()
    {
        if (yenidenDolduruyor)
            yield break;

        if (yedekMermi <= 0)
        {
            Debug.Log("Yedek mermi kalmadı!");
            yield break;
        }

        yenidenDolduruyor = true;
        ReloadSesiCal();
        yield return new WaitForSeconds(reloadSuresi);

        int eksikMermi = sarjorKapasitesi - mevcutMermi;
        int alinacakMermi = Mathf.Min(eksikMermi, yedekMermi);

        mevcutMermi += alinacakMermi;
        yedekMermi = Mathf.Max(0, yedekMermi - alinacakMermi);

        yenidenDolduruyor = false;
        MermiUIGuncelle();
    }

    void ReloadSesiCal()
    {
        if (reloadSesi == null)
            return;

        Vector3 sesPozisyonu = oyuncuKamerasi != null ? oyuncuKamerasi.transform.position : transform.position;
        AudioSource.PlayClipAtPoint(reloadSesi, sesPozisyonu, reloadSesSeviyesi * SettingsManager.SfxVolume);
    }

    void MermiYokSesiCal()
    {
        if (mermiYokSesi == null || Time.time < sonrakiMermiYokSesiZamani)
            return;

        Vector3 sesPozisyonu = oyuncuKamerasi != null ? oyuncuKamerasi.transform.position : transform.position;
        AudioSource.PlayClipAtPoint(mermiYokSesi, sesPozisyonu, mermiYokSesSeviyesi * SettingsManager.SfxVolume);
        sonrakiMermiYokSesiZamani = Time.time + mermiYokSesAraligi;
    }

    void MermiUIGuncelle()
    {
        if (mermiYazisi != null)
        {
            mermiYazisi.text = mevcutMermi + " / " + yedekMermi;

            if (mevcutMermi <= kritikMermiSiniri)
                mermiYazisi.color = kritikMermiRengi;
            else if (mevcutMermi <= azMermiSiniri)
                mermiYazisi.color = azMermiRengi;
            else
                mermiYazisi.color = normalMermiRengi;
        }
    }

    public void YedekMermiEkle(int miktar)
    {
        yedekMermi += miktar;
        MermiUIGuncelle();
    }

    void HitMarkerGoster(bool kafaVurusu)
    {
        if (nisangahEfektiCoroutine != null)
            StopCoroutine(nisangahEfektiCoroutine);

        AudioClip calacakSes = kafaVurusu && headshotSesi != null ? headshotSesi : vurusSesi;

        if (calacakSes != null && oyuncuKamerasi != null)
            AudioSource.PlayClipAtPoint(calacakSes, oyuncuKamerasi.transform.position, vurusSesSeviyesi * SettingsManager.SfxVolume);

        nisangahEfektiCoroutine = StartCoroutine(NisangahVurusEfekti(kafaVurusu));
    }

    IEnumerator NisangahVurusEfekti(bool kafaVurusu)
    {
        nisangahEfektiCalisiyor = true;
        Color baslangicRengi = nisangahGorseli.color;
        Color hedefRenk = kafaVurusu ? headshotRengi : vurusRengi;
        float efektSuresi = Mathf.Max(vurusEfektSuresi, 0.08f);
        float buyumeMiktari = Mathf.Max(vurusBuyumeMiktari, 1.45f);

        if (kafaVurusu)
            buyumeMiktari *= Mathf.Max(headshotBuyumeCarpani, 1f);

        float gecenSure = 0f;

        while (gecenSure < efektSuresi)
        {
            gecenSure += Time.unscaledDeltaTime;
            float oran = Mathf.Clamp01(gecenSure / efektSuresi);
            float vurgu = 1f - oran;
            float scale = Mathf.Lerp(1f, buyumeMiktari, vurgu);

            nisangahGorseli.rectTransform.localScale = nisangahOrijinalScale * scale;
            nisangahGorseli.color = Color.Lerp(baslangicRengi, hedefRenk, vurgu);

            yield return null;
        }

        nisangahGorseli.rectTransform.localScale = nisangahOrijinalScale;
        nisangahGorseli.color = baslangicRengi;
        nisangahEfektiCalisiyor = false;
        nisangahEfektiCoroutine = null;
    }

    [Header("Kovan Ayarları")]
    public GameObject kovanPrefab;
    public Transform kovanCikisNoktasi;
    public float firlatmaKuvveti = 5f;
    public float dondurmeKuvveti = 10f;

    public void KovanFirlat()
    {
        if (kovanPrefab == null || kovanCikisNoktasi == null) return;

        GameObject yeniKovan = Instantiate(kovanPrefab, kovanCikisNoktasi.position, kovanCikisNoktasi.rotation);
        Rigidbody rb = yeniKovan.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(kovanCikisNoktasi.forward * firlatmaKuvveti, ForceMode.Impulse);
            rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * dondurmeKuvveti, ForceMode.Impulse);
        }

        Destroy(yeniKovan, 5f);
    }

    public bool ReloadYapiyorMu()
    {
        return yenidenDolduruyor;
    }
}