using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIBar : MonoBehaviour
{
    [SerializeField] private bool _allowMaxText;
    [SerializeField] private bool _smooth = true;
    [SerializeField] private Image bar;
    [SerializeField] private Image differenceBar;
    [SerializeField] private TextMeshProUGUI valueText;

    public bool Filled => bar.fillAmount >= 0.99f;

    public void SetValue(float value, float maxValue)
    {
        value = Mathf.Clamp(value, 0.0f, maxValue);

        if (differenceBar != null)
            differenceBar.fillAmount = value / maxValue;

        if (value / maxValue < bar.fillAmount)
        {
            bar.DOKill();
            bar.fillAmount = value / maxValue;
        }
        else
        {
            if (_smooth)
                DOVirtual.DelayedCall(0.25f, () => bar.DOFillAmount(value / maxValue, 1.0f));
            else
                bar.fillAmount = value / maxValue;
        }

        if (valueText != null)
        {
            valueText.text = $"{(int)value}/{(int)maxValue}";
            if (_allowMaxText && value >= maxValue)
                valueText.text = "MAX";
        }
    }

    public void SetText(string value)
    {
        if (value == null || valueText == null)
            return;
        valueText.text = value;
    }
}
