using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    public void SetGraphics(int index)
    {
        if (index == 0)
        {
            QualitySettings.SetQualityLevel(0);
        }
        else if (index == 1)
        {
            QualitySettings.SetQualityLevel(2);
        }
        else if (index == 2)
        {
            QualitySettings.SetQualityLevel(5);
        }
    }
}