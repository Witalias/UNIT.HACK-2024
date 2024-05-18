using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlantInfoPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _propertiesText;
    [SerializeField] private UIBar _progressBar;

    private Tween _currentTween;
    private bool _isShown;

    private void Start()
    {
        _panel.alpha = 0.0f;
    }

    public void Show(string title, string properties)
    {
        //if (!_isShown)
        //{
            _isShown = true;
            _titleText.text = title;
            _propertiesText.text = properties;
            _currentTween?.Kill();
            _currentTween = _panel.DOFade(1.0f, 0.25f);
            _progressBar.gameObject.SetActive(false);
        //}
    }

    public void RefreshProgressBar(float progress, float maxProgress)
    {
        _progressBar.gameObject.SetActive(true);
        _progressBar.SetValue(progress, maxProgress);
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
