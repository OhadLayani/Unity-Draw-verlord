using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float HitDamage { get; private set; }

    private void Awake()
    {
        HitDamage = 5f; //TEMPORARY FIXME for now this is the damage but in the future include attack damage calculations that take into account the attacker's attack stat
    }
}
