using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1City");
    }
}