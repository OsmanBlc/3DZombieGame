using UnityEngine;

public class ZombiCan : MonoBehaviour
{
    public float maksimumCan = 100f;
    private float mevcutCan;

    void Start()
    {
        // Zombi doğduğunda canını fullüyoruz
        mevcutCan = maksimumCan;
    }

    // Silahımız zombiyi vurduğunda bu fonksiyonu tetikleyecek
    public void HasarAl(float hasarMiktari)
    {
        mevcutCan -= hasarMiktari;
        Debug.Log(gameObject.name + " vuruldu! Kalan Can: " + mevcutCan);

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }

    void Ol()
    {
        Debug.Log("Zombi ÖLDÜ!");

        // Şimdilik test için zombiyi sahneden siliyoruz. 
        // İleride buraya "yere düşme ve ölme" animasyonunu ekleyeceğiz!
        Destroy(gameObject);
    }
}