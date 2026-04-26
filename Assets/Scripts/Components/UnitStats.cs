using System;
using UnityEditor.UIElements;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public UnitStatProfile profile;

    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public float Damage { get; private set; }
    public float Speed { get; private set; }

    public event Action<float> OnDamageTaken;
    public event Action OnDeath;

    void Awake()
    {
        InitUnitStats(profile);
    }

    public void TakeDamage(float damageAmount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damageAmount, 0, MaxHP);
        Debug.Log($"{damageAmount} damage Taken! new HP is {CurrentHP}");
        OnDamageTaken?.Invoke(damageAmount);
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} says: Man I'm dead");
        OnDeath?.Invoke();
        Destroy(gameObject);

        //other death logic or something
    }

    private void InitUnitStats(UnitStatProfile profile)
    {
        if (profile == null)
        {
            Debug.LogError($"No UnitStatsProfile found on {gameObject.name}!");
            return;
        }

        MaxHP = profile.maxHP;
        CurrentHP = profile.maxHP;
        Damage = profile.damage;
        Speed = profile.speed;
    }

    private void Start()
    {
        Debug.Log(CurrentHP);
    }
}
