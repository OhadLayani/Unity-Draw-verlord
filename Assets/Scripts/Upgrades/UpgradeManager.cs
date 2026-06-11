using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lives on the Player. Tracks all active upgrades and their stack counts.
/// Other systems should talk to this to apply or query upgrades.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    // --- Public drop pool ---------------------------------------------------
    // Populate this list in the inspector with all UpgradeSOs that can drop.
    // The UpgradeChoiceUI will pull from here to build the 3-option menu.
    [Header("Drop Pool")]
    [Tooltip("All upgrades that can appear as drops. Add new SOs here.")]
    public List<UpgradeSO> upgradePool = new List<UpgradeSO>();

    // --- Internal state -----------------------------------------------------
    // Tracks how many stacks of each upgrade the player currently has.
    private readonly Dictionary<UpgradeSO, int> activeUpgrades = new Dictionary<UpgradeSO, int>();

    private PlayerController player;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    /// <summary>
    /// Apply one stack of an upgrade. If already at max stacks, does nothing.
    /// </summary>
    public void ApplyUpgrade(UpgradeSO upgrade)
    {
        if (upgrade == null) return;

        activeUpgrades.TryGetValue(upgrade, out int currentStacks);

        if (currentStacks >= upgrade.maxStacks)
        {
            Debug.Log($"{upgrade.upgradeName} is already at max stacks ({upgrade.maxStacks}).");
            return;
        }

        int newStacks = currentStacks + 1;
        activeUpgrades[upgrade] = newStacks;
        upgrade.Apply(player, newStacks);

        Debug.Log($"Applied {upgrade.upgradeName} — stack {newStacks}/{upgrade.maxStacks}");
    }

    /// <summary>
    /// Swap one upgrade for another, transferring the stack count.
    /// Use this for the tree-switching mechanic:
    /// the new upgrade starts at the same level as the old one.
    /// </summary>
    public void SwapUpgrade(UpgradeSO oldUpgrade, UpgradeSO newUpgrade)
    {
        if (oldUpgrade == null || newUpgrade == null) return;

        activeUpgrades.TryGetValue(oldUpgrade, out int stacksToTransfer);

        // Undo the old upgrade
        if (stacksToTransfer > 0)
        {
            oldUpgrade.Remove(player, stacksToTransfer);
            activeUpgrades.Remove(oldUpgrade);
        }

        // Apply the new upgrade at the transferred stack level
        int clampedStacks = Mathf.Min(stacksToTransfer, newUpgrade.maxStacks);
        if (clampedStacks > 0)
        {
            activeUpgrades[newUpgrade] = clampedStacks;
            newUpgrade.Apply(player, clampedStacks);
        }

        Debug.Log($"Swapped {oldUpgrade.upgradeName} → {newUpgrade.upgradeName} at stack {clampedStacks}");
    }

    /// <summary>
    /// Returns current stack count for a given upgrade (0 if not active).
    /// </summary>
    public int GetStacks(UpgradeSO upgrade)
    {
        activeUpgrades.TryGetValue(upgrade, out int stacks);
        return stacks;
    }

    /// <summary>
    /// Returns true if the player has at least one stack of this upgrade.
    /// </summary>
    public bool HasUpgrade(UpgradeSO upgrade) => GetStacks(upgrade) > 0;

    /// <summary>
    /// Returns true if the upgrade is at max stacks.
    /// </summary>
    public bool IsMaxed(UpgradeSO upgrade) => upgrade != null && GetStacks(upgrade) >= upgrade.maxStacks;
}
