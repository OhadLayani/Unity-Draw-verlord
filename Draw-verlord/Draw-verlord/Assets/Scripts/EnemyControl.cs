using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Chase")]
    public float speed = 3f;
    public float stopDistance = 2.8f;

    [Header("Avoidance")]
    public float avoidanceRadius = 1.8f;
    public float sidewaysStrength = 4f;
    public LayerMask enemyLayer;

    [Header("Noise")]
    public float noiseStrength = 0.15f;
    public float noiseChangeMin = 0.4f;
    public float noiseChangeMax = 1.2f;

    private Transform player;
    private Rigidbody2D rb;

    private Vector2 noiseDirection;
    private float noiseTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("No object with Player tag was found.");
        }

        PickNewNoiseDirection();
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        UpdateNoise();

        Vector2 myPos = rb.position;
        Vector2 playerPos = player.position;

        Vector2 toPlayer = playerPos - myPos;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 moveDir = toPlayer.normalized;
        Vector2 avoidance = GetSidewaysAvoidance(moveDir);

        // Forward movement is reduced a bit so avoidance can actually win.
        Vector2 finalDir = (moveDir * 0.6f) + avoidance + (noiseDirection * noiseStrength);

        if (finalDir.sqrMagnitude > 0.001f)
        {
            finalDir.Normalize();
            rb.linearVelocity = finalDir * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    Vector2 GetSidewaysAvoidance(Vector2 forwardDir)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, enemyLayer);

        Vector2 totalAvoidance = Vector2.zero;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 toOther = (Vector2)hit.transform.position - rb.position;
            float dist = toOther.magnitude;

            if (dist < 0.001f) continue;

            Vector2 dirToOther = toOther / dist;

            // Only sidestep enemies that are generally in front of us.
            float frontDot = Vector2.Dot(forwardDir, dirToOther);

            if (frontDot > 0f)
            {
                Vector2 sideStep = new Vector2(-dirToOther.y, dirToOther.x);

                float crossZ = forwardDir.x * dirToOther.y - forwardDir.y * dirToOther.x;
                float sideSign = Mathf.Sign(crossZ);

                // Prevent 0 sign edge case
                if (sideSign == 0f)
                {
                    sideSign = Random.value < 0.5f ? -1f : 1f;
                }

                sideStep *= sideSign;

                float strength = 1f - Mathf.Clamp01(dist / avoidanceRadius);
                totalAvoidance += sideStep.normalized * strength;
            }
        }

        return totalAvoidance * sidewaysStrength;
    }

    void UpdateNoise()
    {
        noiseTimer -= Time.fixedDeltaTime;

        if (noiseTimer <= 0f)
        {
            PickNewNoiseDirection();
        }
    }

    void PickNewNoiseDirection()
    {
        noiseDirection = Random.insideUnitCircle.normalized;
        noiseTimer = Random.Range(noiseChangeMin, noiseChangeMax);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}