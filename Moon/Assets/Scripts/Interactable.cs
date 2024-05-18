using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Outline _outline;

    private bool _enabled = true;

    private event Action<bool> OnMouseIsOver;

    public bool MouseIsOver { get; private set; }
    public bool Enabled
    {
        get => _enabled;
        set
        {
            //_outline.enabled = value;
            _enabled = value;
            if (!value)
                UiManager.Instance.HideHotkeys();
        }
    }

    private void Start()
    {
        _outline.enabled = false;
    }

    public void SetMouseIsOver(bool value)
    {
        if (!Enabled || MouseIsOver == value) return;
        _outline.enabled = value;
        MouseIsOver = value;
        OnMouseIsOver?.Invoke(value);
    }

    public void SubscribeOnMouseIsOver(Action<bool> action) => OnMouseIsOver += action;

    public void UnsubscribeOnMouseIsOver(Action<bool> action) => OnMouseIsOver -= action;
}
