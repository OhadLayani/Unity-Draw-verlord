using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    private enum ENEMY_STATE
    {
        MOVE,
        STOP,
        OUT_THE_WAY
    }

    [Header("Chase")]
    public float speed = 3f;
    public float stopDistance = 2.8f;

    private ENEMY_STATE currentState = ENEMY_STATE.MOVE;

    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        PlayerMovements playerMovement = FindFirstObjectByType<PlayerMovements>();

        if (playerMovement != null)
        {
            player = playerMovement.transform;
        }
        else
        {
            Debug.LogError("No PlayerMovement found in the scene.");
        }
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        Vector2 myPos = rb.position;
        Vector2 playerPos = player.position;

        Vector2 toPlayer = playerPos - myPos;
        float distanceToPlayer = toPlayer.magnitude;
        // Flip toward player
        if (toPlayer.x >= 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        switch (currentState)
        {
            case ENEMY_STATE.MOVE:
                {
                    if (distanceToPlayer <= stopDistance)
                    {
                        rb.linearVelocity = Vector2.zero;
                        currentState = ENEMY_STATE.STOP;
                        break;
                    }

                    Vector2 moveDir = toPlayer.normalized;
                    rb.linearVelocity = moveDir * speed;
                    break;
                }

            case ENEMY_STATE.STOP:
                {
                    rb.linearVelocity = Vector2.zero;

                    if (distanceToPlayer > stopDistance)
                    {
                        currentState = ENEMY_STATE.MOVE;
                    }

                    break;
                }

            case ENEMY_STATE.OUT_THE_WAY:
                {
                    // Placeholder:
                    // Later this state will move the enemy away from other enemies
                    // to prevent overlapping or crowding.

                    currentState = ENEMY_STATE.MOVE;
                    break;
                }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}