using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.LowLevel;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] private UnitStatProfile profile;
    [SerializeField] private GameObject attackObject;
    public int InkValue { get; protected set; }
    public float MaxHP { get; protected set; }
    public float CurrentHP { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public float AttackCooldown { get; protected set; }
    public float AttackDuration { get; protected set; }
    public bool IsFriendly { get; protected set; }

    protected bool attackReady = true;

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
        if (attackObject != null) { attackObject.SetActive(false); }
        Debug.Log($"{gameObject.name} IsFriendly set to: {profile.isFriendly}");

    }

    //TODO attack constructor 
    public virtual void TakeDamage(float damageAmount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damageAmount, 0, MaxHP);
        Debug.Log($"{gameObject.name} took {damageAmount} damage! new HP is {CurrentHP}"); ;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.ModifyInkCount(InkValue);
        }
        Debug.Log($"{gameObject.name} says: Man I'm dead");
        Destroy(gameObject);
    }

    public virtual void TriggerAttack(Vector3 attackTargetWorldPosition)
    {
        Debug.Log($"TriggerAttack called on {gameObject.name}");


        if (attackObject == null)
        {
            Debug.LogWarning($"Attack object on {gameObject.name} is null, cannot trigger attack");
            return;
        }

        if (!attackReady)
        {
            Debug.Log("Attack not ready");
            return;
        }
        float attackAngle = CalculateAttackAngle(attackTargetWorldPosition);

        attackObject.transform.rotation = Quaternion.Euler(0f, 0f, attackAngle);

        StartCoroutine(AttackRoutine());

        float CalculateAttackAngle(Vector3 attackTargetWorldPosition)
        {
            float angle;

            Vector2 direction = attackTargetWorldPosition - attackObject.transform.position;
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
