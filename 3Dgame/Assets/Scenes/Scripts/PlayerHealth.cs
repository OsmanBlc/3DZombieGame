using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarlari")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public RectTransform healthBarFill;
    public Image healthBarFillImage;
    public TextMeshProUGUI healthText;

    [Header("Can Bar Renkleri")]
    public Color healthColorFull = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color healthColorMid = new Color(0.9f, 0.75f, 0.1f, 1f);
    public Color healthColorLow = new Color(0.9f, 0.15f, 0.1f, 1f);
    public float lowHealthThreshold = 0.3f;
    public float midHealthThreshold = 0.6f;

    [Header("Hasar Ekran Efekti")]
    public Image damageFlashImage;
    public Color damageFlashColor = new Color(1f, 0f, 0f, 0.35f);
    public float damageFlashDuration = 0.22f;
    public float damageFlashCooldown = 0.25f;

    [Header("Iyilestirme Efekti")]
    public Color healFlashColor = new Color(0.1f, 0.9f, 0.2f, 0.3f);

    [Header("Hasar Sesi")]
    public AudioClip playerHurtClip;
    public float playerHurtVolume = 0.8f;
    public float playerHurtCooldown = 0.45f;

    private float fullWidth;
    private float nextDamageFlashTime = 0f;
    private float nextHurtSoundTime = 0f;
    private Coroutine damageFlashCoroutine;
    private Coroutine pulseCoroutine;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarFill != null)
            fullWidth = healthBarFill.sizeDelta.x;

        if (healthBarFillImage == null && healthBarFill != null)
            healthBarFillImage = healthBarFill.GetComponent<Image>();

        // Ağır aramayı sadece oyun başında bir kez yapıyoruz (FPS kurtaran hamle)
        DamageFlashHazirla();
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (damage > 0f)
        {
            FlashEfektiGoster(damageFlashColor, damageFlashDuration);
            HasarSesiCal();
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float miktar)
    {
        if (isDead || miktar <= 0f)
            return;

        currentHealth += miktar;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();
        FlashEfektiGoster(healFlashColor, 0.35f);
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        UpdateHealthUI();
        GameFlowManager.ShowGameOver();
    }

    void UpdateHealthUI()
    {
        if (healthBarFill == null) return; // 🛡️ İlk başta aldığın hata için güvenlik duvarı

        float healthPercent = currentHealth / maxHealth;

        float newWidth = fullWidth * healthPercent;
        healthBarFill.sizeDelta = new Vector2(newWidth, healthBarFill.sizeDelta.y);

        if (healthBarFillImage != null)
        {
            Color hedefRenk;
            if (healthPercent > midHealthThreshold)
                hedefRenk = Color.Lerp(healthColorMid, healthColorFull, (healthPercent - midHealthThreshold) / (1f - midHealthThreshold));
            else if (healthPercent > lowHealthThreshold)
                hedefRenk = Color.Lerp(healthColorLow, healthColorMid, (healthPercent - lowHealthThreshold) / (midHealthThreshold - lowHealthThreshold));
            else
                hedefRenk = healthColorLow;

            healthBarFillImage.color = hedefRenk;
        }

        if (healthText != null)
            healthText.text = Mathf.RoundToInt(currentHealth).ToString();

        // Nabız kontrol mekanizması optimize edildi
        bool dusukCan = healthPercent < lowHealthThreshold && !isDead;
        if (dusukCan && pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(NabizEfekti());
        }
        else if (!dusukCan && pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
            TemizleFlashImage();
        }
    }

    // İşlemciyi sömüren sonsuz döngü temizlendi ve optimize edildi kanka
    IEnumerator NabizEfekti()
    {
        while (true)
        {
            float t = 0f;
            float sure = 0.7f;
            while (t < sure)
            {
                t += Time.unscaledDeltaTime;
                float alpha = Mathf.PingPong(t / (sure * 0.5f), 1f) * 0.22f; // Daha stabil geçiş

                if (damageFlashImage != null)
                {
                    Color renk = damageFlashColor;
                    renk.a = alpha;
                    damageFlashImage.color = renk;
                }
                yield return null;
            }
        }
    }

    void DamageFlashHazirla()
    {
        if (damageFlashImage == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();

            if (canvas != null)
            {
                GameObject flashObject = new GameObject("DamageFlashOverlay");
                flashObject.transform.SetParent(canvas.transform, false);
                flashObject.transform.SetAsLastSibling();

                RectTransform rectTransform = flashObject.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                damageFlashImage = flashObject.AddComponent<Image>();
                damageFlashImage.raycastTarget = false;
            }
        }

        TemizleFlashImage();
    }

    void TemizleFlashImage()
    {
        if (damageFlashImage != null)
        {
            Color temizRenk = damageFlashColor;
            temizRenk.a = 0f;
            damageFlashImage.color = temizRenk;
        }
    }

    void FlashEfektiGoster(Color flashRenk, float sure)
    {
        if (damageFlashImage == null || Time.time < nextDamageFlashTime)
            return;

        nextDamageFlashTime = Time.time + damageFlashCooldown;

        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);

        damageFlashCoroutine = StartCoroutine(FlashCoroutine(flashRenk, sure));
    }

    IEnumerator FlashCoroutine(Color flashRenk, float sure)
    {
        float gecenSure = 0f;
        damageFlashImage.color = flashRenk;

        while (gecenSure < sure)
        {
            gecenSure += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(flashRenk.a, 0f, gecenSure / sure);

            Color yeniRenk = flashRenk;
            yeniRenk.a = alpha;
            damageFlashImage.color = yeniRenk;

            yield return null;
        }

        // Düşük canda nabız efekti devam edebilsin diye eğer can düşükse temizleme işlemini nabza bırakıyoruz
        if ((currentHealth / maxHealth) >= lowHealthThreshold)
        {
            TemizleFlashImage();
        }

        damageFlashCoroutine = null;
    }

    void HasarSesiCal()
    {
        if (playerHurtClip == null || Time.time < nextHurtSoundTime)
            return;

        AudioSource.PlayClipAtPoint(playerHurtClip, transform.position, playerHurtVolume * SettingsManager.SfxVolume);
        nextHurtSoundTime = Time.time + playerHurtCooldown;
    }
}