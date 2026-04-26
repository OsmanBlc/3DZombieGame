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
    private bool yenidenDolduruyor = false;

    [Header("Referanslar")]
    public Camera oyuncuKamerasi;
    public ParticleSystem namluAtesi;
    public AudioSource silahSesi;

    [Header("UI")]
    public TMP_Text mermiYazisi;

    [Header("Nişangah Ayarları")]
    public Image nisangahGorseli;
    public Color normalRenk = Color.white;
    public Color zombiRengi = Color.red;
    public float vurusBuyumeMiktari = 1.3f;
    public float vurusEfektSuresi = 0.08f;

    [Header("Efektler")]
    public GameObject kanEfekti;

    private SilahHissiyat hissiyat;
    private Vector3 nisangahOrijinalScale;
    private bool nisangahEfektiCalisiyor = false;

    void Start()
    {
        if (oyuncuKamerasi == null)
            oyuncuKamerasi = Camera.main;

        if (silahSesi == null)
            silahSesi = GetComponent<AudioSource>();

        hissiyat = GetComponent<SilahHissiyat>();

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
            }
        }
    }

    void NisangahKontrol()
    {
        if (nisangahGorseli == null || oyuncuKamerasi == null)
            return;

        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, menzil))
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
        {
            blowbackScript.ApplyBlowback();
        }

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

        if (hissiyat != null)
        {
            hissiyat.GeriTepmeUygula();
        }
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

        if (Physics.Raycast(ray, out hit, menzil))
        {
            ZombiCan vurulanZombi = hit.transform.GetComponentInParent<ZombiCan>();

            if (vurulanZombi != null)
            {
                float uygulanacakHasar = hasar;

                if (hit.collider.CompareTag("Head"))
                {
                    uygulanacakHasar *= kafaHasarCarpani;
                }

                vurulanZombi.HasarAl(uygulanacakHasar, hit.point);

                if (kanEfekti != null)
                {
                    Instantiate(kanEfekti, hit.point, Quaternion.LookRotation(hit.normal));
                }

                if (!nisangahEfektiCalisiyor && nisangahGorseli != null)
                {
                    StartCoroutine(NisangahVurusEfekti());
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
        yield return new WaitForSeconds(reloadSuresi);

        int eksikMermi = sarjorKapasitesi - mevcutMermi;
        int alinacakMermi = Mathf.Min(eksikMermi, yedekMermi);

        mevcutMermi += alinacakMermi;
        yedekMermi -= alinacakMermi;

        yenidenDolduruyor = false;
        MermiUIGuncelle();
    }

    void MermiUIGuncelle()
    {
        if (mermiYazisi != null)
        {
            mermiYazisi.text = mevcutMermi + " / " + yedekMermi;
        }
    }

    IEnumerator NisangahVurusEfekti()
    {
        nisangahEfektiCalisiyor = true;
        nisangahGorseli.rectTransform.localScale = nisangahOrijinalScale * vurusBuyumeMiktari;
        yield return new WaitForSeconds(vurusEfektSuresi);
        nisangahGorseli.rectTransform.localScale = nisangahOrijinalScale;
        nisangahEfektiCalisiyor = false;
    }
}

