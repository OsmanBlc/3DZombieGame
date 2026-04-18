using UnityEngine;

public class SilahHissiyat : MonoBehaviour
{
    [Header("1. Kamera Çevirme (Sway)")]
    public float swayMiktari = 0.02f;
    public float maxSway = 0.05f;
    public float swayHizi = 8f;

    [Header("2. Yürüme (Bobbing)")]
    public float yurumeHizi = 12f;
    public float yurumeMiktari = 0.015f;

    [Header("3. Silah Sekmesi (Recoil)")]
    public float geriyeSekme = 0.15f;
    public float yukariSekme = 3f;
    public float sekmeDonusHizi = 8f;

    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;

    private Vector3 guncelSekmePoz = Vector3.zero;
    private Quaternion guncelSekmeRot = Quaternion.identity;

    private float bobTimer = 0f;

    void Start()
    {
        baslangicPozisyonu = transform.localPosition;
        baslangicRotasyonu = transform.localRotation;
    }

    void Update()
    {
        // 1. SWAY
        float fareX = -Input.GetAxis("Mouse X") * swayMiktari;
        float fareY = -Input.GetAxis("Mouse Y") * swayMiktari;

        fareX = Mathf.Clamp(fareX, -maxSway, maxSway);
        fareY = Mathf.Clamp(fareY, -maxSway, maxSway);

        Vector3 swayPozisyonu = new Vector3(fareX, fareY, 0f);

        // 2. BOBBING
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");

        Vector3 bobPozisyonu = Vector3.zero;

        if (Mathf.Abs(yatay) > 0.1f || Mathf.Abs(dikey) > 0.1f)
        {
            bobTimer += Time.deltaTime * yurumeHizi;

            bobPozisyonu = new Vector3(
                Mathf.Cos(bobTimer * 0.5f) * yurumeMiktari,
                Mathf.Sin(bobTimer) * yurumeMiktari,
                0f
            );
        }
        else
        {
            bobTimer = 0f;
            bobPozisyonu = Vector3.Lerp(bobPozisyonu, Vector3.zero, Time.deltaTime * swayHizi);
        }

        // 3. RECOIL TOPARLAMA
        guncelSekmePoz = Vector3.Lerp(guncelSekmePoz, Vector3.zero, Time.deltaTime * sekmeDonusHizi);
        guncelSekmeRot = Quaternion.Slerp(guncelSekmeRot, Quaternion.identity, Time.deltaTime * sekmeDonusHizi);

        // HEPSİNİ BİRLEŞTİR
        Vector3 hedefPozisyon = baslangicPozisyonu + swayPozisyonu + bobPozisyonu + guncelSekmePoz;
        Quaternion hedefRotasyon = baslangicRotasyonu * guncelSekmeRot;

        transform.localPosition = Vector3.Lerp(transform.localPosition, hedefPozisyon, Time.deltaTime * swayHizi);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, hedefRotasyon, Time.deltaTime * swayHizi);
    }

    public void GeriTepmeUygula()
    {
        guncelSekmePoz += new Vector3(0f, 0f, -geriyeSekme);
        guncelSekmeRot *= Quaternion.Euler(-yukariSekme, Random.Range(-1f, 1f), 0f);
    }
}
