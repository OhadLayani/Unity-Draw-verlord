using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkCounterUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
   // [SerializeField] private TMP_Text inkText;
    [SerializeField] private Image _image;
    private int maxValue;
    private int currentValue;
    private float targetFill;
    [SerializeField] int LerpSpeed;
    private void Update()
    {
        maxValue = player.maxInkCount;
        currentValue = player.InkCount;
        targetFill = Mathf.Clamp01((float)currentValue / maxValue);

         _image.fillAmount = Mathf.Lerp(_image.fillAmount, targetFill, LerpSpeed * Time.deltaTime);
    }
}