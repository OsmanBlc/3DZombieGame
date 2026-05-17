using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject backgroundDim;

    public void ToggleShop()
    {
        bool isActive = shopPanel.activeSelf;

        shopPanel.SetActive(!isActive);
        backgroundDim.SetActive(!isActive);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        backgroundDim.SetActive(false);
    }
}