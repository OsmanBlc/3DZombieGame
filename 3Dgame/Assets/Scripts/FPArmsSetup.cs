using UnityEngine;

/// <summary>
/// FP Arms kamerasını ve el rigini otomatik kurar.
/// Player -> Main Camera altına bu scripti ekleyin.
/// </summary>
public class FPArmsSetup : MonoBehaviour
{
    [Header("Referanslar")]
    public Camera mainCamera;
    public Transform fpRig;          // El + Silah parent objesi

    [Header("Gizlenecek Karakter")]
    public Renderer[] bodyRenderers; // SK_Military_Survivalist'in tüm renderer'ları

    private Camera armsCamera;

    void Awake()
    {
        // Vücut görünümünü kapat
        foreach (var r in bodyRenderers)
        {
            if (r != null) r.enabled = false;
        }

        SetupArmsCamera();
    }

    void SetupArmsCamera()
    {
        if (mainCamera == null) return;

        // FP_Arms layer'ının index'ini al (Unity'de tanımlamış olmalısınız)
        int fpArmsLayer = LayerMask.NameToLayer("FP_Arms");
        if (fpArmsLayer == -1)
        {
            Debug.LogWarning("FPArmsSetup: 'FP_Arms' layer bulunamadı! Project Settings > Tags and Layers'dan ekleyin.");
            return;
        }

        // Main camera FP_Arms layer'ını görmemelidir
        mainCamera.cullingMask &= ~(1 << fpArmsLayer);

        // Arms camera oluştur (main camera'nın child'ı)
        GameObject armsCamObj = new GameObject("ArmsCamera");
        armsCamObj.transform.SetParent(mainCamera.transform, false);
        armsCamObj.transform.localPosition = Vector3.zero;
        armsCamObj.transform.localRotation = Quaternion.identity;

        armsCamera = armsCamObj.AddComponent<Camera>();
        armsCamera.clearFlags = CameraClearFlags.Depth;
        armsCamera.cullingMask = 1 << fpArmsLayer;
        armsCamera.depth = mainCamera.depth + 1;
        armsCamera.fieldOfView = mainCamera.fieldOfView;
        armsCamera.nearClipPlane = 0.01f;  // Yakın clip - ellerin duvardan geçmemesi için

        // FP Rig'i FP_Arms layer'ına al
        if (fpRig != null)
            SetLayerRecursive(fpRig.gameObject, fpArmsLayer);
    }

    void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}
