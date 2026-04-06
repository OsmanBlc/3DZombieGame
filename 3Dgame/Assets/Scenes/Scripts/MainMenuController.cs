using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;

    // Start Game butonu
    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    // Settings aç
    public void OpenSettings()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // Settings kapat
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    // Oyundan çýk
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatýldý");
    }
}
