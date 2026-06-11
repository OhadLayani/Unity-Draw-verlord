using TMPro;
using UnityEngine;

public class InkCounterUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private TMP_Text inkText;

    private void Update()
    {
        if (player == null || inkText == null)
            return;

        inkText.text = $"{player.InkCount} / {player.maxInkCount}";
    }
}