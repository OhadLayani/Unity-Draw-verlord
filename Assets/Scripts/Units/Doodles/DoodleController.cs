using UnityEngine;
using System;
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

    [Header("Expendable")]
    [SerializeField] private int attacksRemaining = 3;
    [SerializeField] private Hurtbox hurtbox;

    private Transform player;
    private Rigidbody2D rb;

    private Vector2 chargeTargetPosition;
    private float chargeIdleTimer;
    private Hurtbox attackTarget;
    private Hurtbox markedTarget;

    private DoodleState currentState;
    private DucklingState ducklingState;
    private ChargeState chargeState;
    private AttackState attackState;
    public static event Action<DoodleController> OnDoodleDied;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        followOffset = UnityEngine.Random.insideUnitCircle.normalized * followOffsetRadius;

        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        if (playerController != null)
            player = playerController.transform;
        else
            Debug.LogError("No PlayerController found in the scene.");

        ducklingState = new DucklingState(this);
        chargeState = new ChargeState(this);
        attackState = new AttackState(this);

        ChangeState(DOODLE_STATE.DUCKLING);

        if (hurtbox != null)
            hurtbox.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerController.OnDoodleChargeCommand += OnChargeCommand;
        PlayerController.OnEnemyMarked += OnEnemyMarkedCommand;
    }

    private void OnDisable()
    {
        PlayerController.OnDoodleChargeCommand -= OnChargeCommand;
        PlayerController.OnEnemyMarked -= OnEnemyMarkedCommand;
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        currentState?.Tick();
    }

    public override void TriggerAttack(Vector3 attackTargetWorldPosition)
    {
        if (!attackReady) return;

        base.TriggerAttack(attackTargetWorldPosition);

        attacksRemaining--;
        if (attacksRemaining <= 0)
            Die();
    }

    public override void Die()
    {
        OnDoodleDied?.Invoke(this);
        base.Die();
    }

    private void ChangeState(DOODLE_STATE newState)
    {
        currentState?.Exit();

        switch (newState)
        {
            case DOODLE_STATE.DUCKLING:
                currentState = ducklingState;
                break;

            case DOODLE_STATE.CHARGE:
                currentState = chargeState;
                break;

            case DOODLE_STATE.ATTACK:
                currentState = attackState;
                break;
        }

        currentState.Enter();
    }

    private void OnEnemyMarkedCommand(Hurtbox hurtbox)
    {
        Debug.Log($"{name} moving to marked enemy");
        markedTarget = hurtbox;
        attackTarget = hurtbox;
        attackOffset = UnityEngine.Random.insideUnitCircle.normalized * attackSurroundRadius;
        ChangeState(DOODLE_STATE.ATTACK);
    }

    private void OnChargeCommand(Vector2 targetPosition)
    {
        Debug.Log($"{name} charging to position");
        markedTarget = null;
        chargeTargetPosition = targetPosition;
        chargeIdleTimer = idleTimeAtChargeLocation;
        ChangeState(DOODLE_STATE.CHARGE);
    }

    private void ReturnToDuckling()
    {
        rb.linearVelocity = Vector2.zero;
        ChangeState(DOODLE_STATE.DUCKLING);
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

        if (markedTarget != null)
            return;

        attackTarget = hurtbox;
        attackOffset = UnityEngine.Random.insideUnitCircle.normalized * attackSurroundRadius;

        ChangeState(DOODLE_STATE.ATTACK);
    }

    private abstract class DoodleState
    {
        protected DoodleController doodle;

        protected DoodleState(DoodleController doodle)
        {
            this.doodle = doodle;
        }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void Exit() { }
    }

    private class DucklingState : DoodleState
    {
        public DucklingState(DoodleController doodle) : base(doodle) { }

        public override void Enter()
        {
            doodle.spriteRenderer.sprite = doodle.spriteFrames[0];
        }

        public override void Tick()
        {
            Vector2 followPosition = (Vector2)doodle.player.position + doodle.followOffset;
            Vector2 toPlayer = followPosition - (Vector2)doodle.transform.position;

            if (toPlayer.magnitude <= doodle.stopDistance)
            {
                doodle.rb.linearVelocity = Vector2.zero;
                return;
            }

            doodle.rb.linearVelocity = toPlayer.normalized * doodle.Speed;
        }
    }

    private class ChargeState : DoodleState
    {
        public ChargeState(DoodleController doodle) : base(doodle) { }

        public override void Enter()
        {
            doodle.spriteRenderer.sprite = doodle.spriteFrames[1];
        }

        public override void Tick()
        {
            if (!doodle.IsVisibleOnScreen())
            {
                doodle.ReturnToDuckling();
                return;
            }

            Vector2 toTarget = doodle.chargeTargetPosition - (Vector2)doodle.transform.position;

            if (toTarget.magnitude > doodle.chargeStopDistance)
            {
                doodle.rb.linearVelocity = toTarget.normalized * doodle.Speed;
                return;
            }

            doodle.rb.linearVelocity = Vector2.zero;

            doodle.chargeIdleTimer -= Time.fixedDeltaTime;

            if (doodle.chargeIdleTimer <= 0f)
            {
                doodle.ReturnToDuckling();
            }
        }
    }

    private class AttackState : DoodleState
    {
        public AttackState(DoodleController doodle) : base(doodle) { }

        public override void Enter()
        {
            doodle.spriteRenderer.sprite = doodle.spriteFrames[2];
        }

        public override void Tick()
        {
            if (doodle.attackTarget == null)
            {
                doodle.markedTarget = null;
                doodle.ReturnToDuckling();
                return;
            }

            Vector2 targetPosition = (Vector2)doodle.attackTarget.transform.position + doodle.attackOffset;
            Vector2 toTarget = targetPosition - (Vector2)doodle.transform.position;

            if (toTarget.magnitude <= doodle.stopDistance)
            {
                doodle.rb.linearVelocity = Vector2.zero;
                doodle.TriggerAttack(doodle.attackTarget.transform.position);
                return;
            }

            doodle.rb.linearVelocity = toTarget.normalized * doodle.Speed;
        }
    }
}