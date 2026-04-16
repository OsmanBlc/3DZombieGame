using UnityEngine;

public class ZombiCan : MonoBehaviour
{
    public float maksimumCan = 100f;
    private float mevcutCan;

    public Animator animator;
    public float vuruluncaGeriGitme = 0.18f;

    private bool olduMu = false;

    void Start()
    {
        mevcutCan = maksimumCan;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void HasarAl(float hasarMiktari, Vector3 vurulmaNoktasi)
    {
        if (olduMu) return;

        mevcutCan -= hasarMiktari;

        // Hafif vurulma tepkisi
        Vector3 geriYon = (transform.position - vurulmaNoktasi).normalized;
        geriYon.y = 0f;
        transform.position += geriYon * vuruluncaGeriGitme;

        Debug.Log("Zombi hasar aldı. Kalan can: " + mevcutCan);

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }

    void Ol()
    {
        if (olduMu) return;
        olduMu = true;

        if (animator != null)
            animator.SetTrigger("Die");

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        ZombieFollow follow = GetComponent<ZombieFollow>();
        if (follow != null)
            follow.enabled = false;

        Destroy(gameObject, 2.5f);
    }
}