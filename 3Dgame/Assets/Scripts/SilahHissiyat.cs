using UnityEngine;

public class SilahHissiyat : MonoBehaviour
{
    [Header("1. Kamera Çevirme (Sway)")]
    public float swayMiktari = 0.02f;
    public float maxSway = 0.05f;
    public float swayHizi = 4f;

    [Header("2. Yürüme (Bobbing)")]
    public float yurumeHizi = 12f;
    public float yurumeMiktari = 0.015f;

    [Header("3. Silah Sekmesi (Recoil)")]
    public float geriyeSekme = 0.15f;    // Silahın geriye tepme mesafesi
    public float yukariSekme = 3f;       // Silahın namlusunun kalkma açısı
    public float sekmeDonusHizi = 8f;    // Sekmeden sonra eski haline dönme hızı

    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;

    private Vector3 guncelSekmePoz;
    private Quaternion guncelSekmeRot;
    private float bobTimer = 0f;

    void Start()
    {
        // Oyun başladığında silahın kameradaki orijinal yerini hafızaya alıyoruz
        baslangicPozisyonu = transform.localPosition;
        baslangicRotasyonu = transform.localRotation;
    }

    void Update()
    {
        // --- 1. SİLAHI ÇEVİRME (SWAY) ---
        // Fareyi sağa sola çevirdiğimizde silahın ters yönde hafifçe gecikmesi
        float fareX = -Input.GetAxis("Mouse X") * swayMiktari;
        float fareY = -Input.GetAxis("Mouse Y") * swayMiktari;

        fareX = Mathf.Clamp(fareX, -maxSway, maxSway);
        fareY = Mathf.Clamp(fareY, -maxSway, maxSway);
        Vector3 swayPozisyonu = new Vector3(fareX, fareY, 0);

        // --- 2. YÜRÜME (BOBBING) ---
        // WASD tuşlarına basıldığında silahın sonsuzluk işareti (sinüs dalgası) çizerek sallanması
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");
        Vector3 bobPozisyonu = Vector3.zero;

        if (Mathf.Abs(yatay) > 0.1f || Mathf.Abs(dikey) > 0.1f)
        {
            bobTimer += Time.deltaTime * yurumeHizi;
            bobPozisyonu = new Vector3(
                Mathf.Cos(bobTimer / 2) * yurumeMiktari,
                Mathf.Sin(bobTimer) * yurumeMiktari,
                0);
        }
        else
        {
            bobTimer = 0f; // Karakter durduğunda sallanmayı sıfırla
        }

        // --- 3. SEKME DÜZELTME ---
        // Ateş ettikten sonra silahı yumuşakça orijinal pozisyonuna geri çekiyoruz
        guncelSekmePoz = Vector3.Lerp(guncelSekmePoz, Vector3.zero, Time.deltaTime * sekmeDonusHizi);
        guncelSekmeRot = Quaternion.Slerp(guncelSekmeRot, Quaternion.identity, Time.deltaTime * sekmeDonusHizi);

        // --- HEPSİNİ BİRLEŞTİR VE SİLAHA UYGULA ---
        Vector3 hedefPozisyon = baslangicPozisyonu + swayPozisyonu + bobPozisyonu + guncelSekmePoz;
        transform.localPosition = Vector3.Lerp(transform.localPosition, hedefPozisyon, Time.deltaTime * swayHizi);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, baslangicRotasyonu * guncelSekmeRot, Time.deltaTime * sekmeDonusHizi);
    }

    // Bu fonksiyonu diğer scriptten (ateş ettiğimizde) çağıracağız
    public void GeriTepmeUygula()
    {
        // Silahı Z ekseninde geriye it ve X ekseninde yukarı kaldır, Y ekseninde rastgele hafif titret
        guncelSekmePoz += new Vector3(0, 0, -geriyeSekme);
        guncelSekmeRot *= Quaternion.Euler(-yukariSekme, Random.Range(-1f, 1f), 0);
    }
}