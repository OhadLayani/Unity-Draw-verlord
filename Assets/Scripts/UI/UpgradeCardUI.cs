using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One card in the upgrade choice panel.
/// Receives an UpgradeSO and displays its info.
/// Tells UpgradeChoiceUI which upgrade was picked when clicked.
/// </summary>
public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text stackText;   // shows e.g. "Lv 2 / 5"
    [SerializeField] private Button button;

    private UpgradeSO upgrade;
    private UpgradeChoiceUI parentUI;

    // -------------------------------------------------------------------------

    public void Setup(UpgradeSO upgradeSO, UpgradeChoiceUI ui)
    {
        upgrade = upgradeSO;
        parentUI = ui;

        if (iconImage != null)  iconImage.sprite = upgradeSO.icon;
        if (nameText != null)   nameText.text = upgradeSO.upgradeName;
        if (descriptionText != null) descriptionText.text = upgradeSO.description;

        if (stackText != null)
        {
            int current = ui.GetComponent<UpgradeManager>() != null
                ? ui.GetComponent<UpgradeManager>().GetStacks(upgradeSO)
                : 0;
            stackText.text = current > 0
                ? $"Lv {current} → {current + 1} / {upgradeSO.maxStacks}"
                : $"New!  Max: {upgradeSO.maxStacks}";
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        parentUI.OnCardPicked(upgrade);
    }
}
