using UnityEngine;

public class RangedEnemyControl : UnitBase
{
    private enum RANGED_STATE
    {
        MOVE,
        STOP
    }

    [Header("Chase")]
    public float speed = 3f;
    public float stopDistance = 6f;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 2f;

    private RANGED_STATE currentState = RANGED_STATE.MOVE;
    private float fireTimer;

    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        if (playerController != null)
            player = playerController.transform;
        else
            Debug.LogError("No PlayerController found in the scene.");

        fireTimer = fireCooldown;
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        Vector2 myPos = rb.position;
        Vector2 playerPos = player.position;

        Vector2 toPlayer = playerPos - myPos;
        float distanceToPlayer = toPlayer.magnitude;

        transform.rotation = toPlayer.x >= 0
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.Euler(0f, 0f, 0f);

        switch (currentState)
        {
            case RANGED_STATE.MOVE:
                if (distanceToPlayer <= stopDistance)
                {
                    rb.linearVelocity = Vector2.zero;
                    currentState = RANGED_STATE.STOP;
                    break;
                }

                rb.linearVelocity = toPlayer.normalized * speed;
                break;

            case RANGED_STATE.STOP:
                rb.linearVelocity = Vector2.zero;

                if (distanceToPlayer > stopDistance)
                {
                    currentState = RANGED_STATE.MOVE;
                    break;
                }

                fireTimer -= Time.fixedDeltaTime;
                if (fireTimer <= 0f)
                {
                    FireAtPlayer(toPlayer);
                    fireTimer = fireCooldown;
                }
                break;
        }
    }

    private void FireAtPlayer(Vector2 directionToPlayer)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.GetComponent<EnemyProjectile>().Init(directionToPlayer, this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
