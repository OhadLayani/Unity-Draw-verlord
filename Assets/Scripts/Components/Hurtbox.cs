using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private UnitBase unit;

    public bool HurtboxIsFriendly  { get; private set; }

    private void Awake()
    {
        if (unit == null)
        {
            Debug.LogError("No Unit component asssigned on hurtbox, hurtbox will not work properly and likely crash the game");
        }
        HurtboxIsFriendly = unit.isFriendly;
    }

    public void TriggerDamageTaken(float hitDamage)
    {
        unit.TakeDamage(hitDamage);
    }
}
