using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemType _type;
    [SerializeField] private Interactable _interactable;

    public static event Action OnPicked;

    private void Start()
    {
        _interactable.SubscribeOnMouseIsOver(OnMouseIsOver);
    }

    private void Update()
    {
        if (_interactable.MouseIsOver && Input.GetKeyDown(KeyCode.E))
            PickUp();
    }

    private void OnMouseIsOver(bool value)
    {
        if (value)
        {
            UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "Подобрать" });
        }
        else
        {
            UiManager.Instance.HideHotkeys();
        }
    }

    private void PickUp()
    {
        Inventory.Instance.Add(_type);
        UiManager.Instance.HideHotkeys();
        UiManager.Instance.HideObjectInfo();
        Destroy(gameObject);
        OnPicked?.Invoke();
    }
}
