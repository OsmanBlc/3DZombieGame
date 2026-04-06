using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 1.5f;
    public float stopDistance = 1.8f;

    // 🔥 Yeni ekledikler
    public float attackCooldown = 1.5f;

    private Animator anim;
    private float attackTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        attackTimer = 0f;
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
}