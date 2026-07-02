using Microsoft.Unity.VisualStudio.Editor;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkCounterUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private TMP_Text inkText;
    [SerializeField] private UnityEngine.UI.Image inkmeter;
    private void Update()
    {
        if (player == null || inkText == null)
            return;

        inkText.text = $"{player.InkCount} / {player.maxInkCount}";
        inkmeter.fillAmount = (float)Mathf.Clamp01((float)player.InkCount / player.maxInkCount);
    }
}