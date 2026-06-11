using UnityEngine;

/// <summary>
/// Spawned in the world when an enemy dies.
/// When the player walks into it, it triggers the upgrade choice UI.
///
/// To spawn a drop: call UpgradeDrop.Spawn(position, choiceUI) from UnitBase.Die().
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class UpgradeDrop : MonoBehaviour
{
    private UpgradeChoiceUI choiceUI;

    // -------------------------------------------------------------------------

    /// <summary>
    /// Convenience factory. Spawns a drop at the given position.
    /// Call this from UnitBase.Die() instead of Instantiate-ing directly.
    /// </summary>
    public static void Spawn(GameObject prefab, Vector2 position, UpgradeChoiceUI ui)
    {
        if (prefab == null || ui == null) return;

        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        go.GetComponent<UpgradeDrop>()?.Init(ui);
    }

    public void Init(UpgradeChoiceUI ui)
    {
        choiceUI = ui;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        if (choiceUI == null) return;
        if (choiceUI.IsOpen) return; // don't open two menus at once

        choiceUI.Open();
        Destroy(gameObject);
    }
}
