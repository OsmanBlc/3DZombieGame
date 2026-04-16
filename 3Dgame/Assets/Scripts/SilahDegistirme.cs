using UnityEngine;

public class SilahDegistirme : MonoBehaviour
{
    public GameObject[] silahlar;
    private int aktifSilahIndex = 0;

    void Start()
    {
        SilahiAktifEt(aktifSilahIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SilahiAktifEt(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SilahiAktifEt(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SilahiAktifEt(2);
        }
    }

    void SilahiAktifEt(int index)
    {
        if (index < 0 || index >= silahlar.Length)
            return;

        aktifSilahIndex = index;

        for (int i = 0; i < silahlar.Length; i++)
        {
            silahlar[i].SetActive(i == index);
        }
    }
}