using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarlari")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public RectTransform healthBarFill;
    public TextMeshProUGUI healthText;

    private float fullWidth;

    void Start()
    {
        currentHealth = maxHealth;
        fullWidth = healthBarFill.sizeDelta.x;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Can: " + currentHealth);
        UpdateHealthUI();
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
}