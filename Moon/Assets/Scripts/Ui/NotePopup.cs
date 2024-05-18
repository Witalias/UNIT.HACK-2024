using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotePopup : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private CanvasGroup _content;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _image;

    private Vector3 _defaultContentPosition;

    private void Awake()
    {
        _defaultContentPosition = _content.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _container.activeSelf)
            Hide();
    }

    public void Show(string text, bool activeImage)
    {
        DOVirtual.DelayedCall(Time.deltaTime, () => _container.SetActive(true));
        _text.text = text;
        Time.timeScale = 0.0f;
        _content.transform.position = _defaultContentPosition - new Vector3(0.0f, 50.0f);
        _content.DOFade(0.0f, 0.0f);
        DOTween.Sequence()
            .Append(_content.transform.DOMoveY(_defaultContentPosition.y, 1.0f))
            .Insert(0, _content.DOFade(1.0f, 1.0f))
            .SetUpdate(true)
            .Play();
        _image.SetActive(activeImage);
    }

    private void Hide()
    {
        _container.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
