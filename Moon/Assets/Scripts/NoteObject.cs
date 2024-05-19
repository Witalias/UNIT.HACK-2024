using System;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField][Multiline] private string _text;
    [SerializeField] private bool _activeImage;
    [SerializeField] private Interactable _interactable;

    public static event Action NoteCollected;

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
        UiManager.Instance.HideObjectInfo();
        AudioManager.Instanse.Play(AudioType.Paper);
        UiManager.Instance.ShowNote(_text, _activeImage);
        NoteCollected?.Invoke();
        Destroy(gameObject);
    }
}
