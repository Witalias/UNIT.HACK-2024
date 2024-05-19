using System;
using UnityEngine;

public class EdibleWildPlant : MonoBehaviour
{
    [SerializeField] private ItemType _seedType;
    [SerializeField] private Interactable _interactable;

    public bool ContainSeed { get; private set; } = true;

    public static event Action OnPicked;

    private void Start()
    {
        _interactable.SubscribeOnMouseIsOver(OnMouseIsOver);
    }

    private void Update()
    {
        if (_interactable.MouseIsOver && Input.GetKeyDown(KeyCode.E))
            GetSeed();
    }

    private void OnMouseIsOver(bool value)
    {
        if (value)
        {
            UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "Извлечь семя" });
        }
        else
        {
            UiManager.Instance.HideHotkeys();
        }
    }

    private void GetSeed()
    {
        ContainSeed = false;
        Inventory.Instance.Add(_seedType);
        UiManager.Instance.HideHotkeys();
        UiManager.Instance.HideObjectInfo();
        AudioManager.Instanse.Play(AudioType.PickUp);
        OnPicked?.Invoke();
    }
}
