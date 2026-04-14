using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private UnitStats stats;

    private void Awake()
    {
        if (stats == null)
        {
            Debug.LogError("No UnitStats component found, hurtbox will not work properly and likely crash the game");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("detected collision");
        if (other.TryGetComponent<Hitbox>(out var hitbox))
        {
            stats.TakeDamage(hitbox.HitDamage);
        }
    }
}
