using UnityEngine;

public class FPSBodySetup : MonoBehaviour
{
    [Header("Karakter Objesi")]
    public GameObject characterRoot; // SK_Military_Survivalist

    [Header("FPS Arms")]
    public SkinnedMeshRenderer fpsArmsRenderer; // SK_Miliary_FPS_Arms

    [Header("Silah Holder")]
    public Transform weaponHolder;
    public string handBoneName = "hand_r";
    public Vector3 weaponLocalPosition = new Vector3(0f, 0f, 0f);
    public Vector3 weaponLocalRotation = new Vector3(0f, 0f, 0f);

    void Awake()
    {
        if (characterRoot != null)
        {
            // Tüm Skinned Mesh Renderer'ları bul ve kapat (FPS Arms hariç)
            SkinnedMeshRenderer[] allRenderers = characterRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in allRenderers)
            {
                if (smr == fpsArmsRenderer)
                {
                    smr.enabled = true; // FPS Arms açık kalsın
                    Debug.Log("FPS Arms AÇIK: " + smr.name);
                }
                else
                {
                    smr.enabled = false; // Diğer tüm mesh'ler kapalı
                    Debug.Log("Gizlendi: " + smr.name);
                }
            }
        }

        AttachWeaponToHand();
    }

    void AttachWeaponToHand()
    {
        if (fpsArmsRenderer == null || weaponHolder == null) return;

        foreach (Transform bone in fpsArmsRenderer.bones)
        {
            if (bone != null && bone.name == handBoneName)
            {
                weaponHolder.SetParent(bone);
                weaponHolder.localPosition = weaponLocalPosition;
                weaponHolder.localRotation = Quaternion.Euler(weaponLocalRotation);
                Debug.Log("Silah bağlandı: " + bone.name);
                return;
            }
        }

        Debug.LogWarning("Hand kemiği bulunamadı: " + handBoneName);
    }
}
