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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SelectCell(false);
        if (Input.GetKeyDown(KeyCode.RightArrow)) SelectCell(true);
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
                return;
            } 
        }
        UpdateNameText();
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
