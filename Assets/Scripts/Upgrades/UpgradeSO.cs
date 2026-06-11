using UnityEngine;

/// <summary>
/// Base class for all upgrades. Create a new upgrade by making a ScriptableObject
/// that inherits from this and overrides Apply / Remove.
///
/// Apply is called every time the player picks this upgrade (once per stack level).
/// Remove is called when the player swaps away from this upgrade tree —
/// used to undo the effect so the new upgrade can be applied cleanly.
/// </summary>
public abstract class UpgradeSO : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Stacking")]
    public int maxStacks = 5;

    /// <summary>
    /// Called when this upgrade is applied or gains a stack.
    /// currentStacks is the NEW stack count after this pick (starts at 1).
    /// </summary>
    public abstract void Apply(PlayerController player, int currentStacks);

    /// <summary>
    /// Called when this upgrade is swapped out for another in the same slot.
    /// Use this to undo whatever Apply did.
    /// </summary>
    public abstract void Remove(PlayerController player, int currentStacks);
}
