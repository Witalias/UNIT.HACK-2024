using DG.Tweening;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private Cell[] _cells;
    [SerializeField] private Transform _selectionTransform;
    [SerializeField] private TMP_Text _nameText;
    //[SerializeField] private GameObject _hotkeysText;

    private int _currentSelectedCellIndex;
    private Tween _currentTween;

    public ItemType? SelectedItem
    {
        get
        {
            if (_cells[_currentSelectedCellIndex].Count <= 0)
                return null;
            return _cells[_currentSelectedCellIndex].Item;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DOTween.Sequence()
            .Append(_selectionTransform.DOScale(1.02f, 0.25f))
            .Append(_selectionTransform.DOScale(1.0f, 0.25f))
            .SetLoops(-1)
            .Play();
        _nameText.text = "";
        Add(ItemType.Plant1Seed);
        Add(ItemType.Plant1Seed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SelectCell(false);
        if (Input.GetKeyDown(KeyCode.RightArrow)) SelectCell(true);
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCell(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCell(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCell(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCell(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectCell(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectCell(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectCell(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectCell(7);
    }

    public void Add(ItemType item)
    {
        foreach (var cell in _cells)
        {
            if (cell.Count > 0 && cell.Item == item)
            {
                cell.Add();
                return;
            }
        }
        foreach (var cell in _cells)
        {
            if (cell.Count <= 0)
            {
                cell.SetItem(item);
                UpdateNameText();
                return;
            } 
        }
    }

    public bool TryPeek(ItemType item)
    {
        foreach (var cell in _cells)
        {
            if (cell.Count > 0 && cell.Item == item)
            {
                cell.Reduce();
                UpdateNameText();
                return true;
            }
        }
        return false;
    }

    public bool IsExist(ItemType item)
    {
        foreach (var cell in _cells)
        {
            if (cell.Count > 0 && cell.Item == item)
                return true;
        }
        return false;
    }

    public bool IsSelect(ItemType item)
    {
        return _cells[_currentSelectedCellIndex].Count > 0 && _cells[_currentSelectedCellIndex].Item == item;
    }

    public bool IsSelect(ItemType[] items)
    {
        if (_cells[_currentSelectedCellIndex].Count <= 0)
            return false;
        foreach (var item in items)
        {
            if (_cells[_currentSelectedCellIndex].Item == item)
                return true;
        }
        return false;
    }

    private void SelectCell(bool next)
    {
        if (next)
        {
            if (++_currentSelectedCellIndex >= _cells.Length)
                _currentSelectedCellIndex = 0;
        }
        else
        {
            if (--_currentSelectedCellIndex < 0)
                _currentSelectedCellIndex = _cells.Length - 1;
        }
        //_hotkeysText.SetActive(_cells[_currentSelectedCellIndex].Count > 0);
        UpdateNameText();
        _currentTween?.Kill();
        _currentTween = _selectionTransform.DOMove(_cells[_currentSelectedCellIndex].transform.position, 0.1f);
    }

    private void SelectCell(int index)
    {
        _currentSelectedCellIndex = index;
        UpdateNameText();
        _currentTween?.Kill();
        _currentTween = _selectionTransform.DOMove(_cells[_currentSelectedCellIndex].transform.position, 0.1f);
    }

    private void UpdateNameText()
    {
        if (_cells[_currentSelectedCellIndex].Count > 0)
            _nameText.text = GameData.Instance.GetItemData(_cells[_currentSelectedCellIndex].Item).Name;
        else
            _nameText.text = string.Empty;
    }
}

public enum ItemType
{
    Water,
    Fertilizer,
    Plant1,
    Plant1Seed,
    Plant2,
    Plant2Seed,
    Plant3,
    Plant3Seed,
}
