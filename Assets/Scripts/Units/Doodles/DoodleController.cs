using UnityEditor.U2D;
using UnityEngine;

public class DoodleController : UnitBase
{
    private enum DOODLE_STATE
    {
        DUCKLING,
        CHARGE,
        ATTACK
    }
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] spriteFrames;

    [Header("Chase")]
    public float stopDistance = 2.8f;

    [Header("Follow Offset")]
    [SerializeField] private float followOffsetRadius = 1.5f;
    private Vector2 followOffset;

    [Header("Attack Surround")]
    [SerializeField] private float attackSurroundRadius = 1.2f;
    private Vector2 attackOffset;

    [Header("Charge")]
    public float chargeStopDistance = 0.2f;
    public float idleTimeAtChargeLocation = 10f;

    private DOODLE_STATE currentState = DOODLE_STATE.DUCKLING;

    private Transform player;
    private Rigidbody2D rb;

    private Vector2 chargeTargetPosition;
    private float chargeIdleTimer;
    private Hurtbox attackTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        followOffset = Random.insideUnitCircle.normalized * followOffsetRadius;

        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        if (playerController != null)
        {
            player = playerController.transform;
        }
        else
        {
            Debug.LogError("No PlayerController found in the scene.");
        }
    }

    private void OnEnable()
    {
        PlayerController.OnDoodleChargeCommand += OnChargeCommand;
    }

    private void OnDisable()
    {
        PlayerController.OnDoodleChargeCommand -= OnChargeCommand;
    }

    private void OnChargeCommand(Vector2 targetPosition)
    {
        chargeTargetPosition = targetPosition;
        chargeIdleTimer = idleTimeAtChargeLocation;
        currentState = DOODLE_STATE.CHARGE;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        Vector2 followPosition = (Vector2)player.position + followOffset;
        Vector2 toPlayer = followPosition - (Vector2)transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        switch (currentState)
        {
            case DOODLE_STATE.DUCKLING:
                {
                    if (distanceToPlayer <= stopDistance)
                    {
                        rb.linearVelocity = Vector2.zero;
                        break;
                    }

                    Vector2 moveDir = toPlayer.normalized;
                    rb.linearVelocity = moveDir * Speed;

                    spriteRenderer.sprite = spriteFrames[0];
                    break;
                }

            case DOODLE_STATE.CHARGE:
                {
                    if (!IsVisibleOnScreen())
                    {
                        ReturnToDuckling();
                        break;
                    }

                    spriteRenderer.sprite = spriteFrames[1];

                    Vector2 toTarget = chargeTargetPosition - (Vector2)transform.position;
                    float distanceToTarget = toTarget.magnitude;

                    if (distanceToTarget > chargeStopDistance)
                    {
                        Vector2 moveDir = toTarget.normalized;
                        rb.linearVelocity = moveDir * Speed;
                    }
                    else
                    {
                        rb.linearVelocity = Vector2.zero;

                        chargeIdleTimer -= Time.fixedDeltaTime;

                        if (chargeIdleTimer <= 0f)
                        {
                            ReturnToDuckling();
                        }
                    }

                    break;
                }

            case DOODLE_STATE.ATTACK:
                {
                    if (attackTarget == null)
                    {
                        currentState = DOODLE_STATE.DUCKLING;
                        break;
                    }

                    spriteRenderer.sprite = spriteFrames[2];

                    Vector2 targetPosition = (Vector2)attackTarget.transform.position + attackOffset;
                    Vector2 toTarget = targetPosition - (Vector2)transform.position;
                    float distanceToTarget = toTarget.magnitude;

                    if (distanceToTarget <= stopDistance)
                    {
                        rb.linearVelocity = Vector2.zero;

                        //Debug.Log("ATTACK");
                        TriggerAttack(attackTarget.transform.position);

                        break;
                    }

                    Vector2 moveDir = toTarget.normalized;
                    rb.linearVelocity = moveDir * Speed;

                    break;
                }
        }
    }

    private void ReturnToDuckling()
    {
        rb.linearVelocity = Vector2.zero;
        currentState = DOODLE_STATE.DUCKLING;
    }

    private bool IsVisibleOnScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x >= 0f &&
               viewportPos.x <= 1f &&
               viewportPos.y >= 0f &&
               viewportPos.y <= 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();

        if (hurtbox == null)
            return;

        if (hurtbox.HurtboxIsFriendly == IsFriendly)
            return;

        attackTarget = hurtbox;
        attackOffset = Random.insideUnitCircle.normalized * attackSurroundRadius;
        currentState = DOODLE_STATE.ATTACK;

        //Debug.Log("Enemy detected");
    }
}