using System.Collections;
using UnityEngine;

public class ZombiCan : MonoBehaviour
{
    public float maksimumCan = 100f;
    private float mevcutCan;

    public Animator animator;
    public float vuruluncaGeriGitme = 0.18f;

    [Header("Ses")]
    public AudioClip zombieHurtClip;
    public float zombieHurtVolume = 0.8f;
    public float zombieHurtCooldown = 0.2f;

    [Header("Ekonomi Ödülleri")]
    [Tooltip("Zombi öldüğünde cüzdana tek seferde eklenecek toplu para miktarı")]
    public int olumOduluPara = 150; // Tek seferde toplu geleceği için ödülü bir tık yükselttim kanka, keyfine göre değiştirirsin

    private bool olduMu = false;
    private float sonrakiHurtSesZamani = 0f;
    private SilahDegistirici silahManager;

    void Start()
    {
        mevcutCan = maksimumCan;

        if (animator == null)
            animator = GetComponent<Animator>();

        // Sahnede parayı tutan script'i otomatik buluyoruz aga
        silahManager = FindFirstObjectByType<SilahDegistirici>();
    }

    public void HasarAl(float hasarMiktari, Vector3 vurulmaNoktasi)
    {
        if (olduMu) return;

        mevcutCan -= hasarMiktari;
        ZombieHurtSesiCal();

        // 🌟 DEĞİŞİKLİK: Vuruş başına para ekleme satırı buradan tamamen kaldırıldı kanka!

        // Hafif vurulma tepkisi
        Vector3 geriYon = (transform.position - vurulmaNoktasi).normalized;
        geriYon.y = 0f;
        transform.position += geriYon * vuruluncaGeriGitme;

        Debug.Log("Zombi hasar aldı. Kalan can: " + mevcutCan);

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }

    void Ol()
    {
        if (olduMu) return;
        olduMu = true;

        BolumManager.ZombieOlduruldu();

        // 🎯 TOPLU ÖDÜL SİSTEMİ: Oyuncu zombiyi tamamen indirdiği an parayı tek seferde kasaya yüklüyoruz kanka!
        if (silahManager != null)
        {
            silahManager.ParaEkle(olumOduluPara);
        }

        if (animator != null)
            animator.SetTrigger("Die");

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        ZombieFollow follow = GetComponent<ZombieFollow>();
        if (follow != null)
            follow.enabled = false;

        Destroy(gameObject, 2.5f);
    }

    void ZombieHurtSesiCal()
    {
        if (zombieHurtClip == null || Time.time < sonrakiHurtSesZamani)
            return;

        AudioSource.PlayClipAtPoint(zombieHurtClip, transform.position, zombieHurtVolume * SettingsManager.SfxVolume);
        sonrakiHurtSesZamani = Time.time + zombieHurtCooldown;
    }
}