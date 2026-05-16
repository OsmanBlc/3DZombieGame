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

    [Header("3. GERÇEKÇİ SEKME (Recoil)")]
    [Tooltip("Silahın ateş anında aniden fırlayacağı AÇI (Derece)")]
    public Vector3 sekmeRotasyonu = new Vector3(-12f, 0f, 0f);
    [Tooltip("Silahın ateş anında aniden gideceği POZİSYON (Y: Yukarı, Z: Geri)")]
    public Vector3 sekmePozisyonu = new Vector3(0f, 0.08f, -0.06f);

    [Header("Sekme Yay Hızları (Aşırı Kritik!)")]
    [Tooltip("Silahın yukarı zıplama hızı (Ne kadar yüksekse o kadar ani ve sert vurur)")]
    public float snappiness = 35f;
    [Tooltip("Silahın eski haline dönme hızı (Tok bir his için 10-15 arası ideal)")]
    public float returnSpeed = 14f;

    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;

    // Hedeflenen anlık sekme değerleri
    private Vector3 targetRecoilRotation;
    private Vector3 targetRecoilPosition;

    // Yumuşatılmış anlık sekme değerleri
    private Vector3 currentRecoilRotation;
    private Vector3 currentRecoilPosition;

    private float bobTimer = 0f;

    void Start()
    {
        baslangicPozisyonu = transform.localPosition;
        baslangicRotasyonu = transform.localRotation;
    }

    void Update()
    {
        // --- 1. SWAY (Sallanma) MANTIĞI ---
        float fareX = -Input.GetAxis("Mouse X") * swayMiktari;
        float fareY = -Input.GetAxis("Mouse Y") * swayMiktari;

        fareX = Mathf.Clamp(fareX, -maxSway, maxSway);
        fareY = Mathf.Clamp(fareY, -maxSway, maxSway);

        Vector3 swayPozisyonu = new Vector3(fareX, fareY, 0f);

        // --- 2. BOBBING (Yürüme) MANTIĞI ---
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

        // --- 3. TOK VE YUKARI FIRLAYAN SEKME MOTORU ---
        // Hedefleri sıfıra (orijinal yerine) doğru returnSpeed hızıyla çekiyoruz
        targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        targetRecoilPosition = Vector3.Lerp(targetRecoilPosition, Vector3.zero, returnSpeed * Time.deltaTime);

        // Anlık değerleri hedefe doğru SNAPPINESS (Zıplama) hızıyla fırlatıyoruz
        currentRecoilRotation = Vector3.Lerp(currentRecoilRotation, targetRecoilRotation, snappiness * Time.deltaTime);
        currentRecoilPosition = Vector3.Lerp(currentRecoilPosition, targetRecoilPosition, snappiness * Time.deltaTime);

        // --- 4. HEPSİNİ BİRLEŞTİR (ÇİFT KATMANLI YAY) ---
        Vector3 finalPozisyon = baslangicPozisyonu + swayPozisyonu + bobPozisyonu + currentRecoilPosition;
        Quaternion finalRotasyon = baslangicRotasyonu * Quaternion.Euler(currentRecoilRotation);

        // Pozisyon anında tepki verecek (snappiness hızıyla), rotasyon yumuşakça takip edecek (swayHizi ile)
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPozisyon, Time.deltaTime * snappiness);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotasyon, Time.deltaTime * swayHizi);
    }

    // SilahAtes scriptinden çağrılan fonksiyon
    public void GeriTepmeUygula(bool otomatikSilah)
    {
        float carpantor = otomatikSilah ? 0.65f : 1f;

        // Sadece yukarı kalkış açısı (X ekseni)
        targetRecoilRotation += new Vector3(sekmeRotasyonu.x * carpantor, 0f, 0f);

        // ELLERİN FİZİKSEL OLARAK YUKARI VE GERİYE FIRLAMASI
        targetRecoilPosition += new Vector3(
            0f,                           // Sağa sola kayma yok
            sekmePozisyonu.y * carpantor, // Eli ve silahı yukarı kaldıran değer (Y)
            sekmePozisyonu.z * carpantor  // Eli göğse doğru iten değer (Z)
        );
    }
}