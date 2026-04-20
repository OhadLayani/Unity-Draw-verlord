using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsProfile", menuName = "Units/Unit Stat Profile")]
public class UnitStatProfile : ScriptableObject
{
    public float maxHP = 10;
    public float speed = 2;
    public float damage = 5;
}
