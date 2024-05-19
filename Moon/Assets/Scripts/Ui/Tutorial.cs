using DG.Tweening;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private float _timeToHide;
    [SerializeField] private CanvasGroup _panel;

    private void Start()
    {
        _panel.alpha = 0.0f;
        _panel.DOFade(0.9f, 1.0f);
        DOVirtual.DelayedCall(_timeToHide, () => _panel.DOFade(0.0f, 1.0f));
    }
}
