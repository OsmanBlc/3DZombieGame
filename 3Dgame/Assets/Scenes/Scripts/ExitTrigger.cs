using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Level1Manager.hasKeycard)
        {
            Debug.Log("Level Complete!");
            SceneManager.LoadScene("LevelSelect");
        }
    }
}