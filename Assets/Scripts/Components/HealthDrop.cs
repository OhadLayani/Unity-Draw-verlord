using UnityEngine;

/// <summary>
/// Heals the player on contact.
/// Spawn this by assigning the prefab on UnitBase — it handles the drop chance.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HealthDrop : MonoBehaviour
{
    [SerializeField] private float healAmount = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Don't pick up if already at full health
        if (player.CurrentHP >= player.MaxHP) return;

        player.Heal(healAmount);
        Destroy(gameObject);
    }
}
