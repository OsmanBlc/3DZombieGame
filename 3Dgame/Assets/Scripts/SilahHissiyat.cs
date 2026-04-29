using UnityEngine;

public class SilahHissiyat : MonoBehaviour
{
    [Header("1. Kamera Çevirme (Sway)")]
    public float swayMiktari = 0.015f;
    public float maxSway = 0.03f;
    public float swayHizi = 10f;

    [Header("2. Yürüme (Bobbing)")]
    public float yurumeHizi = 10f;
    public float yurumeMiktari = 0.01f;

    [Header("3. Silah Sekmesi (Recoil)")]
    public float geriyeSekme = 0.04f;
    public float yukariSekme = 1.2f;
    public float sekmeDonusHizi = 14f;
    public float yataySekme = 0.4f;
    public float rollSekme = 0.8f;

    [Header("4. Otomatik Silah Titremesi")]
    public float otomatikSekmeCarpani = 1.35f;
    public float otomatikTitremePozisyonu = 0.018f;
    public float otomatikTitremeDonusu = 1.3f;
    public float otomatikDonusHiziCarpani = 1.8f;

    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;

    private Vector3 guncelSekmePoz = Vector3.zero;
    private Quaternion guncelSekmeRot = Quaternion.identity;
    private float anlikDonusHiziCarpani = 1f;

    private float bobTimer = 0f;

    void Start()
    {
        baslangicPozisyonu = transform.localPosition;
        baslangicRotasyonu = transform.localRotation;
    }

    void Update()
    {
        float fareX = -Input.GetAxis("Mouse X") * swayMiktari;
        float fareY = -Input.GetAxis("Mouse Y") * swayMiktari;

        fareX = Mathf.Clamp(fareX, -maxSway, maxSway);
        fareY = Mathf.Clamp(fareY, -maxSway, maxSway);

        Vector3 swayPozisyonu = new Vector3(fareX, fareY, 0f);

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
        }

        float sekmeHizi = sekmeDonusHizi * anlikDonusHiziCarpani;
        guncelSekmePoz = Vector3.Lerp(guncelSekmePoz, Vector3.zero, Time.deltaTime * sekmeHizi);
        guncelSekmeRot = Quaternion.Slerp(guncelSekmeRot, Quaternion.identity, Time.deltaTime * sekmeHizi);
        anlikDonusHiziCarpani = Mathf.Lerp(anlikDonusHiziCarpani, 1f, Time.deltaTime * sekmeDonusHizi);

        Vector3 hedefPozisyon = baslangicPozisyonu + swayPozisyonu + bobPozisyonu + guncelSekmePoz;
        Quaternion hedefRotasyon = baslangicRotasyonu * guncelSekmeRot;

        transform.localPosition = Vector3.Lerp(transform.localPosition, hedefPozisyon, Time.deltaTime * swayHizi);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, hedefRotasyon, Time.deltaTime * swayHizi);
    }

    public void GeriTepmeUygula()
    {
        GeriTepmeUygula(false);
    }

    public void GeriTepmeUygula(bool otomatikSilah)
    {
        float sekmeCarpani = otomatikSilah ? otomatikSekmeCarpani : 1f;
        float titremePozisyonu = otomatikSilah ? otomatikTitremePozisyonu : 0f;
        float titremeDonusu = otomatikSilah ? otomatikTitremeDonusu : 0f;

        guncelSekmePoz += new Vector3(
            Random.Range(-titremePozisyonu, titremePozisyonu),
            Random.Range(-titremePozisyonu * 0.5f, titremePozisyonu),
            -geriyeSekme * sekmeCarpani
        );

        guncelSekmeRot *= Quaternion.Euler(
            -yukariSekme * sekmeCarpani + Random.Range(-titremeDonusu, titremeDonusu),
            Random.Range(-yataySekme - titremeDonusu, yataySekme + titremeDonusu),
            Random.Range(-rollSekme - titremeDonusu, rollSekme + titremeDonusu)
        );

        if (otomatikSilah)
            anlikDonusHiziCarpani = otomatikDonusHiziCarpani;
    }
}
