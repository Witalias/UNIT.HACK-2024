using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private Hotkey[] _hotkeys;
    [SerializeField] private LayerMask _excludePlayerLayers;
    [SerializeField] private PlantInfoPopup _plantInfoPopup;
    [SerializeField] private ObjectInfoPopup _objectInfoPopup;

    private Camera _mainCamera;
    private Interactable _previousInteracrable;

    private void Awake()
    {
        Instance = this;
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        StartCoroutine(CheckMouseOver());
    }

    public void ShowHotkeys(params Hotkey.Data[] hotkeys)
    {
        HideHotkeys();
        for (var i = 0; i < hotkeys.Length; i++)
        {
            _hotkeys[i].SetData(hotkeys[i]);
            _hotkeys[i].gameObject.SetActive(true);
        }
    }

    public void HideHotkeys()
    {
        foreach (var hotkey in _hotkeys)
        {
            hotkey.gameObject.SetActive(false);
        }
    }

    public void ShowPlantInfo(string title, string properties)
    {
        _plantInfoPopup.Show(title, properties);
    }

    public void RefreshPlantInfo(Vector3 position, float progress, float maxProgress)
    {
        RefreshPlantInfo(position);
        _plantInfoPopup.RefreshProgressBar(progress, maxProgress);
    }

    public void RefreshPlantInfo(Vector3 position)
    {
        _plantInfoPopup.transform.position = _mainCamera.WorldToScreenPoint(position) + new Vector3(0.0f, 20.0f);
    }

    public void HidePlantInfo()
    {
        _plantInfoPopup.Hide();
    }

    public void ShowObjectInfo(string title, string description)
    {
        _objectInfoPopup.Show(title, description);
    }

    public void HideObjectInfo()
    {
        _objectInfoPopup.Hide();
    }

    public void RefreshObjectInfoPosition(Vector3 position)
    {
        _objectInfoPopup.transform.position = _mainCamera.WorldToScreenPoint(position);
    }

    private IEnumerator CheckMouseOver()
    {
        var wait = new WaitForSeconds(0.05f);
        while (true)
        {
            var ray = _mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hit, 100.0f, _excludePlayerLayers))
            {
                if (hit.collider.TryGetComponent<Interactable>(out var interactable))
                {
                    if (interactable != _previousInteracrable)
                    {
                        _previousInteracrable = interactable;
                        interactable.SetMouseIsOver(true);
                    }
                }
                else if (_previousInteracrable != null)
                {
                    _previousInteracrable.SetMouseIsOver(false);
                    _previousInteracrable = null;
                }
            }
            yield return wait;
        }
    }
}
