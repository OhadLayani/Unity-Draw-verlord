using UnityEngine;

/// <summary>
/// Attach to any enemy. Handles loot drops when the unit dies.
/// Listens to UnitBase's OnDeath event — no need to touch enemy scripts.
///
/// To add a new drop type: add a new DropEntry to the drops array in the inspector.
/// </summary>
public class DropOnDeath : MonoBehaviour
{
    [System.Serializable]
    public struct DropEntry
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float chance;
    }

    [SerializeField] private DropEntry[] drops;

    private void Awake()
    {
        UnitBase unit = GetComponent<UnitBase>();
        if (unit == null)
        {
            Debug.LogWarning($"DropOnDeath on {gameObject.name} requires a UnitBase component.");
            return;
        }

        unit.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        foreach (DropEntry drop in drops)
        {
            if (drop.prefab != null && Random.value <= drop.chance)
                Instantiate(drop.prefab, transform.position, Quaternion.identity);
        }
    }
}
