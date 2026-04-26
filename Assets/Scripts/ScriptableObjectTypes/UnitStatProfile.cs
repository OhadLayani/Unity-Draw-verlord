using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsProfile", menuName = "Units/Unit Stat Profile")]
public class UnitStatProfile : ScriptableObject
{
    public float maxHP = 10f;
    public float speed = 2f;
    public float damage = 5f;
    public float attackCooldown = 0.35f;
    public bool isFriendly = false;
}
