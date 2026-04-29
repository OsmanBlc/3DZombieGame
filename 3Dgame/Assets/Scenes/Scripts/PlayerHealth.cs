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
    public TextMeshProUGUI healthText;

    [Header("Hasar Ekran Efekti")]
    public Image damageFlashImage;
    public Color damageFlashColor = new Color(1f, 0f, 0f, 0.35f);
    public float damageFlashDuration = 0.22f;
    public float damageFlashCooldown = 0.25f;

    [Header("Hasar Sesi")]
    public AudioClip playerHurtClip;
    public float playerHurtVolume = 0.8f;
    public float playerHurtCooldown = 0.45f;

    private float fullWidth;
    private float nextDamageFlashTime = 0f;
    private float nextHurtSoundTime = 0f;
    private Coroutine damageFlashCoroutine;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        fullWidth = healthBarFill.sizeDelta.x;
        DamageFlashHazirla();
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Can: " + currentHealth);
        UpdateHealthUI();

        if (damage > 0f)
        {
            DamageFlashGoster();
            HasarSesiCal();
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;
        UpdateHealthUI();
        GameFlowManager.ShowGameOver();
    }

    void UpdateHealthUI()
    {
        float healthPercent = currentHealth / maxHealth;

        float newWidth = fullWidth * healthPercent;
        healthBarFill.sizeDelta = new Vector2(newWidth, healthBarFill.sizeDelta.y);

        if (healthText != null)
        {
            healthText.text = Mathf.RoundToInt(currentHealth).ToString();
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

        if (damageFlashImage != null)
        {
            Color temizRenk = damageFlashColor;
            temizRenk.a = 0f;
            damageFlashImage.color = temizRenk;
        }
    }

    void DamageFlashGoster()
    {
        if (damageFlashImage == null)
            DamageFlashHazirla();

        if (damageFlashImage == null || Time.time < nextDamageFlashTime)
            return;

        nextDamageFlashTime = Time.time + damageFlashCooldown;

        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);

        damageFlashCoroutine = StartCoroutine(DamageFlashEfekti());
    }

    IEnumerator DamageFlashEfekti()
    {
        float gecenSure = 0f;
        damageFlashImage.color = damageFlashColor;

        while (gecenSure < damageFlashDuration)
        {
            gecenSure += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(damageFlashColor.a, 0f, gecenSure / damageFlashDuration);

            Color yeniRenk = damageFlashColor;
            yeniRenk.a = alpha;
            damageFlashImage.color = yeniRenk;

            yield return null;
        }

        Color temizRenk = damageFlashColor;
        temizRenk.a = 0f;
        damageFlashImage.color = temizRenk;
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
