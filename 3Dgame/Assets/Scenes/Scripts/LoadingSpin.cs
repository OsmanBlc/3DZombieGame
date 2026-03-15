using UnityEngine;

public class LoadingSpin : MonoBehaviour
{
    public string nextSceneName = "MainMenu";
    public float speed = 200f;

    void Update()
    {
        transform.Rotate(0, 0, -speed * Time.deltaTime);
    }
}
