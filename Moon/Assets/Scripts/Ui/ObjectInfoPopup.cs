using DG.Tweening;
using TMPro;
using UnityEngine;

public class ObjectInfoPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    private Tween _currentTween;
    private bool _isShown;

    private void Start()
    {
        _panel.alpha = 0.0f;
    }

    public void Show(string title, string description)
    {
        if (!_isShown)
        {
            _isShown = true;
            _titleText.text = title;
            _descriptionText.text = description;
            _currentTween?.Kill();
            _currentTween = _panel.DOFade(1.0f, 0.25f);
        }
    }

    public void Show(string title, string description, bool edible)
    {
        title += $"\n{(edible ? "<color=green>Съедобно" : "<color=red>Несъедобно")}</color>";
        Show(title, description);
    }

    public void Hide()
    {
        if (_isShown)
        {
            _isShown = false;
            _currentTween?.Kill();
            _currentTween = _panel.DOFade(0.0f, 0.25f);
        }
    }
}
