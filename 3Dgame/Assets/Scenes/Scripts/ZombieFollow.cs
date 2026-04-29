using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 1.5f;
    public float stopDistance = 1.8f;

    // 🔥 Yeni ekledikler
    public float attackCooldown = 1.5f;

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

        // Oyuncuya bak
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);

        if (distance > stopDistance)
        {
            // 🟢 YÜRÜME
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            anim.SetBool("isWalking", true);
            ZombieFootstepSesiCal(distance);
        }
        else
        {
            // 🔴 SALDIRI
            anim.SetBool("isWalking", false);

            if (attackTimer <= 0f)
            {
                anim.SetTrigger("Attack");
                attackTimer = attackCooldown;
            }
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
}
