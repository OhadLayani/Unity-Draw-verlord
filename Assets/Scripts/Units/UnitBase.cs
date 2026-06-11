using System.Collections;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] private UnitStatProfile profile;
    [SerializeField] private GameObject attackObject;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject inkOrbPrefab;

    public int InkValue { get; protected set; }
    public float MaxHP { get; protected set; }
    public float CurrentHP { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public float AttackCooldown { get; protected set; }
    public float AttackDuration { get; protected set; }
    public bool IsFriendly { get; protected set; }

    protected bool attackReady = true;

    private Vector3 originalScale;
    private Coroutine hitEffectRoutine;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    void Awake()
    {
        if (profile == null)
        {
            Debug.LogError($"No UnitStatsProfile found on {gameObject.name}! unit will not function properly");
            return;
        }

        MaxHP = profile.maxHP;
        CurrentHP = profile.maxHP;
        Damage = profile.damage;
        Speed = profile.speed;
        AttackCooldown = profile.attackCooldown;
        AttackDuration = profile.attackDuration;
        IsFriendly = profile.isFriendly;
        InkValue = profile.inkValue;

        originalScale = transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        if (attackObject != null)
        {
            attackObject.SetActive(false);
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHP, MaxHP);
        }
    }

    //TODO attack constructor
    public virtual void TakeDamage(float damageAmount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damageAmount, 0, MaxHP);

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHP, MaxHP);
        }

        if (hitEffectRoutine != null)
        {
            StopCoroutine(hitEffectRoutine);
            transform.localScale = originalScale;
        }

        hitEffectRoutine = StartCoroutine(HitEffectRoutine());

        //Debug.Log($"{gameObject.name} took {damageAmount} damage! new HP is {CurrentHP}");

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffectRoutine()
    {
        transform.localScale = new Vector3(
            originalScale.x * 1.2f,
            originalScale.y * 0.8f,
            originalScale.z
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(0.08f);

        transform.localScale = originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    public virtual void Die()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null && inkOrbPrefab != null)
        {
            for (int i = 0; i < InkValue; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 0.6f;
                GameObject orb = Instantiate(inkOrbPrefab, (Vector2)transform.position + offset, Quaternion.identity);
                InkOrb inkOrb = orb.GetComponent<InkOrb>();
                if (inkOrb != null)
                    inkOrb.Init(player, Random.Range(0.1f, 0.4f));
                else
                    Destroy(orb);
            }
        }
        else if (player != null)
        {
            // Fallback if no orb prefab is assigned — grants ink directly
            player.ModifyInkCount(InkValue);
        }

        Destroy(gameObject);
    }

    public virtual void TriggerAttack(Vector3 attackTargetWorldPosition)
    {
        //Debug.Log($"TriggerAttack called on {gameObject.name}");

        if (attackObject == null)
        {
            //Debug.LogWarning($"Attack object on {gameObject.name} is null, cannot trigger attack");
            return;
        }

        if (!attackReady)
        {
            //Debug.Log("Attack not ready on {gameObject.name}");
            return;
        }

        float attackAngle = CalculateAttackAngle(attackTargetWorldPosition);

        attackObject.transform.rotation = Quaternion.Euler(0f, 0f, attackAngle);

        StartCoroutine(AttackRoutine());

        float CalculateAttackAngle(Vector3 _attackTargetWorldPosition)
        {
            float angle;

            Vector2 direction = _attackTargetWorldPosition - attackObject.transform.position;

            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            return angle;
        }
    }

    protected IEnumerator AttackRoutine()
    {
        //Debug.Log("started Attack");

        attackReady = false;

        attackObject.SetActive(true);

        yield return new WaitForSeconds(AttackDuration);

        attackObject.SetActive(false);

        yield return new WaitForSeconds(AttackCooldown);

        attackReady = true;
    }
}