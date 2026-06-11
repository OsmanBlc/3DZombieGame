using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 1.5f;
    public float stopDistance = 1.8f;

    // 👁️ Zombinin oyuncuyu fark etme mesafesi
    [Header("Detection Settings")]
    public float chaseDistance = 40f;

    // 🔥 Saldırı Ayarları
    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public float damageDelay = 0.4f; // Saldırı animasyonu başladıktan kaç saniye sonra hasar vurulsun?

    [Header("Footstep")]
    public AudioClip zombieFootstepClip;
    public AudioSource zombieFootstepSource;
    public float zombieFootstepVolume = 0.55f;
    public float zombieFootstepInterval = 0.65f;
    public float zombieFootstepMaxDistance = 18f;

    private Animator anim;
    private float attackTimer;
    private float nextFootstepTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        attackTimer = 0f;

        if (zombieFootstepSource == null)
        {
            zombieFootstepSource = gameObject.AddComponent<AudioSource>();
            zombieFootstepSource.playOnAwake = false;
            zombieFootstepSource.spatialBlend = 1f;
            zombieFootstepSource.minDistance = 1.5f;
            zombieFootstepSource.maxDistance = zombieFootstepMaxDistance;
        }
    }

    void Update()
    {
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        // 🎯 ŞART 1: Oyuncu takip mesafesinin DIŞINDAYSA zombi sakin kalır (Idle)
        if (distance > chaseDistance)
        {
            anim.SetBool("isWalking", false);
            return;
        }

        // Eğer buraya geçtiyse zombi oyuncuyu fark etmiştir:
        // Oyuncuya bak
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);

        // 🎯 ŞART 2: Oyuncu takip mesafesinde AMA saldırı mesafesinden UZAKTAYSA (YÜRÜME)
        if (distance > stopDistance)
        {
            // 🟢 YÜRÜME
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            anim.SetBool("isWalking", true);
            ZombieFootstepSesiCal(distance);
        }
        // 🎯 ŞART 3: Oyuncu dibindeyse (SALDIRI)
        else
        {
            // 🔴 SALDIRI
            anim.SetBool("isWalking", false);

            if (attackTimer <= 0f)
            {
                anim.SetTrigger("Attack");
                attackTimer = attackCooldown;

                // Animasyon başladıktan 'damageDelay' saniye sonra HasarUygula fonksiyonunu çağırır
                Invoke(nameof(HasarUygula), damageDelay);
            }
        }
    }

    void HasarUygula()
    {
        // Vuruş anında oyuncu hala zombinin yakınındaysa hasar ver (Oyuncu kaçmış olabilir)
        if (player != null && Vector3.Distance(transform.position, player.position) <= stopDistance + 0.5f)
        {
            Debug.Log("Zombi oyuncuya hasar vurdu!");

            // Oyuncunun can scripti hangisiyse buradaki yorum satırını kaldırıp bağlayabilirsin:
            // player.GetComponent<PlayerHealth>().TakeDamage(10); 
        }
    }

    void ZombieFootstepSesiCal(float distance)
    {
        if (zombieFootstepClip == null || zombieFootstepSource == null)
            return;

        if (distance > zombieFootstepMaxDistance || Time.time < nextFootstepTime)
            return;

        zombieFootstepSource.pitch = Random.Range(0.9f, 1.08f);
        zombieFootstepSource.PlayOneShot(zombieFootstepClip, zombieFootstepVolume * SettingsManager.SfxVolume);
        nextFootstepTime = Time.time + zombieFootstepInterval;
    }

    // 🛠️ Sahne ekranında zombinin görüş alanını çizgi olarak görebilmek için
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}