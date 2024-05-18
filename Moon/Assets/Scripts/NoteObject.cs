using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField][Multiline] private string _text;
    [SerializeField] private bool _activeImage;
    [SerializeField] private Interactable _interactable;

    private void Start()
    {
        _interactable.SubscribeOnMouseIsOver(OnMouseIsOver);
    }

    private void Update()
    {
        if (_interactable.MouseIsOver && Input.GetKeyDown(KeyCode.E))
            Read();
    }

    private void OnMouseIsOver(bool value)
    {
        if (value)
        {
            UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "Прочитать" });
        }
        else
        {
            UiManager.Instance.HideHotkeys();
        }
    }

    private void Read()
    {
        UiManager.Instance.HideHotkeys();
        UiManager.Instance.ShowNote(_text, _activeImage);
        AudioManager.Instanse.Play(AudioType.Paper);
        Destroy(gameObject);
    }
}
