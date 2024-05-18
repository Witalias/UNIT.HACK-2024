using System;
using UnityEngine;

public class InteractableWithInfo : Interactable
{
    public static event Func<bool> PlayerIsAim;

    [SerializeField] private string _title;
    [SerializeField] private string _description;

    private bool _isShown;

    private void Update()
    {
        if (MouseIsOver)
        {
            var playerIsAim = PlayerIsAim?.Invoke();
            if (playerIsAim.GetValueOrDefault())
            {
                if (!_isShown)
                {
                    _isShown = true;
                    UiManager.Instance.ShowObjectInfo(_title, _description);
                }
                UiManager.Instance.RefreshObjectInfoPosition(transform.position);
            }
            else if (_isShown)
            {
                _isShown = false;
                UiManager.Instance.HideObjectInfo();
            }
        }
        else if (_isShown)
        {
            _isShown = false;
            UiManager.Instance.HideObjectInfo();
        }
    }
}
