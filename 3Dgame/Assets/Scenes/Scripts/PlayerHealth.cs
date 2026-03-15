using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public Slider healthBar;
    public GameObject gameOverPanel;

    void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            health -= 10;
            healthBar.value = health;

            if (health <= 0)
            {
                health = 0;
                healthBar.value = health;

                if (gameOverPanel != null)
                    gameOverPanel.SetActive(true);

                Time.timeScale = 0f;
            }
        }
    }
}
