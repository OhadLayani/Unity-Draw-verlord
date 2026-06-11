using UnityEngine;

/// <summary>
/// Example upgrade: increases the player's max ink capacity by a flat amount per stack.
///
/// To make a new upgrade:
///   1. Create a new script that inherits UpgradeSO
///   2. Override Apply() and Remove()
///   3. Right-click in Project > Create > [your menu path] to make the SO asset
///   4. Drag the asset into UpgradeManager's pool in the inspector
/// </summary>
[CreateAssetMenu(menuName = "Upgrades/Ink Capacity")]
public class InkCapacityUpgrade : UpgradeSO
{
    [Tooltip("How much max ink is added per stack.")]
    public int inkPerStack = 2;

    public override void Apply(PlayerController player, int currentStacks)
    {
        // Each stack adds inkPerStack to the max.
        // We only add the delta for this stack, not the full total.
        player.maxInkCount += inkPerStack;
    }

    public override void Remove(PlayerController player, int currentStacks)
    {
        // Undo the full cumulative effect when swapping trees.
        player.maxInkCount -= inkPerStack * currentStacks;
    }
}
