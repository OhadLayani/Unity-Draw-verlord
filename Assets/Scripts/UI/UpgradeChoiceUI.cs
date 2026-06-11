using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows 3 upgrade options when the player picks up a drop.
/// Pauses the game while open.
///
/// Hook up in the inspector:
///   - cardRoot: parent object containing the 3 UpgradeCardUI children
///   - upgradeManager: reference to the UpgradeManager on the player
/// </summary>
public class UpgradeChoiceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private UpgradeCardUI[] cards; // exactly 3
    [SerializeField] private UpgradeManager upgradeManager;

    public bool IsOpen { get; private set; }

    // -------------------------------------------------------------------------

    private void Awake()
    {
        panelRoot.SetActive(false);
    }

    /// <summary>
    /// Opens the choice panel. Picks 3 upgrades from the pool at random,
    /// skipping any that are already maxed out.
    /// </summary>
    public void Open()
    {
        List<UpgradeSO> pool = upgradeManager.upgradePool;
        List<UpgradeSO> available = new List<UpgradeSO>(pool.Count);

        foreach (UpgradeSO upgrade in pool)
        {
            if (!upgradeManager.IsMaxed(upgrade))
                available.Add(upgrade);
        }

        if (available.Count == 0)
        {
            Debug.Log("No upgrades available — pool is empty or all maxed.");
            return;
        }

        // Shuffle and take up to 3
        Shuffle(available);
        int optionCount = Mathf.Min(cards.Length, available.Count);

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < optionCount)
            {
                cards[i].Setup(available[i], this);
                cards[i].gameObject.SetActive(true);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }

        IsOpen = true;
        panelRoot.SetActive(true);
        Time.timeScale = 0f; // pause
    }

    /// <summary>
    /// Called by UpgradeCardUI when the player clicks a card.
    /// </summary>
    public void OnCardPicked(UpgradeSO chosen)
    {
        upgradeManager.ApplyUpgrade(chosen);
        Close();
    }

    private void Close()
    {
        IsOpen = false;
        panelRoot.SetActive(false);
        Time.timeScale = 1f; // resume
    }

    // Fisher-Yates shuffle — no LINQ, no allocations beyond the list we already made
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
