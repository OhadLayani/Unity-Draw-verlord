using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsProfile", menuName = "Units/Unit Stats Profile")]
public class UnitStatsProfile : ScriptableObject
{
    public float maxHP = 10;
    public float speed = 2;
    public float damage = 5;
}
