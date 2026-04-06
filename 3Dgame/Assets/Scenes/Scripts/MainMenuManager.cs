using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject buttonsPanel;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(true);
    }
}