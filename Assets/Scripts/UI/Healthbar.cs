using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image hpFillImage;

    public void SetHealth(float currentHP, float maxHP)
    {
        if (hpFillImage == null)
            return;

        if (maxHP <= 0)
            return;

        hpFillImage.fillAmount = Mathf.Clamp01(currentHP / maxHP);
    }
}