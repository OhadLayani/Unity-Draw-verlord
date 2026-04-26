using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsProfile", menuName = "Units/Unit Stat Profile")]
public class UnitStatProfile : ScriptableObject
{
    public float maxHP = 10f;
    public float speed = 2f;
    public float damage = 5f;
    public float attackCooldown = 0.35f;
    public float attackDuration = 0.15f; //TODO remove this once attacks become more self contained rather than unit contained
    public bool isFriendly = false;
}
