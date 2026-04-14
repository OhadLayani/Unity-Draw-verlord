using System;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public UnitStatsProfile profile;

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
        //other death logic or something
    }

    private void InitUnitStats(UnitStatsProfile profile)
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

    //TEMPORARY TESTING this is here to print the hp every once in a while to test damage
    private void Start()
    {
        Debug.Log(CurrentHP);
    }
    private int counter;
    private void Update()
    {
        counter++;
        if (counter%1000 == 0)
        {
            counter -= 1000;
            Debug.Log(CurrentHP);
        }
    }
}
