using UnityEngine;

public class WeaponBoneAttacher : MonoBehaviour
{
    [Header("Referanslar")]
    public SkinnedMeshRenderer armsRenderer;   // SK_Miliary_FPS_Arms
    public Transform weaponHolder;             // WeaponHolder

    [Header("Ayarlar")]
    [Tooltip("El kemiğinin adının bir parçası (büyük/küçük harf farketmez)")]
    public string handBoneKeyword = "hand_r";

    void Start()
    {
        if (armsRenderer == null)
        {
            Debug.LogError("WeaponBoneAttacher: armsRenderer atanmadı!");
            return;
        }

        // Tüm kemik isimlerini konsola yaz
        Debug.Log("=== FPS Arms Kemik Listesi ===");
        foreach (Transform bone in armsRenderer.bones)
        {
            if (bone != null)
                Debug.Log(bone.name);
        }

        // El kemiğini bul ve silahı bağla
        AttachWeapon();
    }

    void AttachWeapon()
    {
        if (weaponHolder == null) return;

        foreach (Transform bone in armsRenderer.bones)
        {
            if (bone != null && bone.name.ToLower().Contains(handBoneKeyword.ToLower()))
            {
                weaponHolder.SetParent(bone);
                weaponHolder.localPosition = Vector3.zero;
                weaponHolder.localRotation = Quaternion.identity;
                Debug.Log("Silah bağlandı: " + bone.name);
                return;
            }
        }

        Debug.LogWarning("El kemiği bulunamadı! Keyword: '" + handBoneKeyword + "' — Konsoldaki listeden doğru ismi bulup scriptte güncelleyin.");
    }
}
