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
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private NotePopup _notePopup;

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

    public void AddHotkey(Hotkey.Data hotkeyData)
    {
        foreach (var hotkey in _hotkeys)
        {
            if (!hotkey.gameObject.activeSelf)
            {
                hotkey.SetData(hotkeyData);
                hotkey.gameObject.SetActive(true);
                return;
            }
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

    public void ShowObjectInfo(string title, string description, bool edible, bool extracted = false)
    {
        _objectInfoPopup.Show(title, description, edible, extracted);
    }

    public void HideObjectInfo()
    {
        _objectInfoPopup.Hide();
    }

    public void RefreshObjectInfoPosition(Vector3 position)
    {
        _objectInfoPopup.transform.position = _mainCamera.WorldToScreenPoint(position) + new Vector3(-200.0f, 400.0f);
    }

    public void AddHealth(float value)
    {
        _healthBar.Add(value);
    }

    public void ShowNote(string text, bool activeImage = false, bool showRestart = false)
    {
        _notePopup.Show(text, activeImage, showRestart);
    }

    private IEnumerator CheckMouseOver()
    {
        var wait = new WaitForSeconds(0.05f);
        while (true)
        {
            var ray = _mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hit, 20.0f, _excludePlayerLayers))
            {
                if (hit.collider.TryGetComponent<Interactable>(out var interactable))
                {
                    if (interactable != _previousInteracrable)
                    {
                        if (_previousInteracrable != null)
                            _previousInteracrable.SetMouseIsOver(false);
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
            else if (_previousInteracrable != null)
            {
                _previousInteracrable.SetMouseIsOver(false);
                _previousInteracrable = null;
            }
            yield return wait;
        }
    }
}
