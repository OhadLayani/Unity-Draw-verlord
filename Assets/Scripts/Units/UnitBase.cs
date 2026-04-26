using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField]
    private UnitStatProfile profile;

    public float MaxHP { get; protected set; }
    public float CurrentHP { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public float AttackCooldown { get; protected set; }
    public bool isFriendly { get; protected set; }

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
        isFriendly = profile.isFriendly;
    }

    //TODO attack constructor 
    protected virtual void TakeDamage(float damageAmount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damageAmount, 0, MaxHP);
        Debug.Log($"{gameObject.name} took {damageAmount} damage! new HP is {CurrentHP}"); ;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} says: Man I'm dead");
    }

    protected virtual void TriggerAttack()
    {
        //coroutine to start attack cooldown
    }
}
