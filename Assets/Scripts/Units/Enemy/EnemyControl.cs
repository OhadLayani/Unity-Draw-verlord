using UnityEngine;

/// <summary>
/// Melee enemy controller.
/// States are nested private classes — same pattern as EnemySpawner.
///
/// To add a new state:
///   1. Add a new private class below that extends EnemyStateBase
///   2. Instantiate it in Start()
///   3. Call TransitionTo(newState) from wherever the transition should happen
/// </summary>
public class EnemyFollow : UnitBase
{
    [Header("Chase")]
    public float StopDistance = 2.8f;

    [Header("Separation (Avoidance)")]
    public float SeparationRadius = 1.2f;
    public float SeparationWeight = 1.2f;
    public LayerMask EnemyLayerMask;

    [Header("Noise")]
    public float NoiseFrequency = 0.8f;
    public float NoiseStrength = 0.15f;

    private Rigidbody2D rb;
    private Transform player;

    private EnemyStateBase currentState;
    private MoveState moveState;
    private StopState stopState;

    // -------------------------------------------------------------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
            player = playerController.transform;
        else
            Debug.LogError("EnemyFollow: No PlayerController found in scene.");

        moveState = new MoveState(this);
        stopState = new StopState(this);

        TransitionTo(moveState);
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // Flip sprite to face the player — shared across all states
        Vector2 toPlayer = (Vector2)player.position - rb.position;
        transform.rotation = toPlayer.x >= 0f
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.Euler(0f, 0f, 0f);

        currentState?.FixedUpdate();
    }

    private void TransitionTo(EnemyStateBase newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, StopDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SeparationRadius);
    }

    // -------------------------------------------------------------------------
    // States
    // -------------------------------------------------------------------------

    private abstract class EnemyStateBase
    {
        protected EnemyFollow enemy;
        public EnemyStateBase(EnemyFollow enemy) { this.enemy = enemy; }
        public virtual void Enter() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }

    private class MoveState : EnemyStateBase
    {
        public MoveState(EnemyFollow enemy) : base(enemy) { }

        public override void FixedUpdate()
        {
            Vector2 myPos = enemy.rb.position;
            Vector2 toPlayer = (Vector2)enemy.player.position - myPos;
            float distanceToPlayer = toPlayer.magnitude;

            if (distanceToPlayer <= enemy.StopDistance)
            {
                enemy.rb.linearVelocity = Vector2.zero;
                enemy.TransitionTo(enemy.stopState);
                return;
            }

            Vector2 desiredDir = toPlayer.normalized;

            // Separation: push away from nearby enemies
            Vector2 separation = Vector2.zero;
            Collider2D[] neighbors = Physics2D.OverlapCircleAll(myPos, enemy.SeparationRadius, enemy.EnemyLayerMask);
            foreach (Collider2D col in neighbors)
            {
                if (col.gameObject == enemy.gameObject) continue;
                Vector2 away = myPos - (Vector2)col.transform.position;
                float dist = away.magnitude;
                if (dist > 0f)
                    separation += away.normalized / dist;
            }

            // Perlin noise to break symmetry and prevent circle-strafing
            float noiseX = (Mathf.PerlinNoise(Time.time * enemy.NoiseFrequency, 0f) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(0f, Time.time * enemy.NoiseFrequency) - 0.5f) * 2f;
            Vector2 noise = new Vector2(noiseX, noiseY) * enemy.NoiseStrength;

            Vector2 finalDir = (desiredDir + separation * enemy.SeparationWeight + noise).normalized;
            enemy.rb.linearVelocity = finalDir * enemy.Speed;
        }
    }

    private class StopState : EnemyStateBase
    {
        public StopState(EnemyFollow enemy) : base(enemy) { }

        public override void Enter()
        {
            enemy.rb.linearVelocity = Vector2.zero;
        }

        public override void FixedUpdate()
        {
            float dist = Vector2.Distance(enemy.rb.position, enemy.player.position);
            if (dist > enemy.StopDistance)
                enemy.TransitionTo(enemy.moveState);
        }
    }
}
