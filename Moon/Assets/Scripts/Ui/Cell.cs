using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TMP_Text _countText;

    public ItemType Item { get; private set; }
    public int Count { get; private set; }

    public void SetItem(ItemType item)
    {
        Item = item;
        Count = 1;
        _countText.gameObject.SetActive(true);
        _itemImage.enabled = true;
        _itemImage.sprite = GameData.Instance.GetItemData(item).Sprite;
    }

    public void Add()
    {
        Count++;
        _countText.text = Count.ToString();
    }

    public void Reduce()
    {
        if (Count > 0)
        {
            Count--;
            if (Count == 0)
            {
                _itemImage.enabled = false;
                _countText.gameObject.SetActive(false);
            }
            else
            {
                _countText.text = Count.ToString();
            }
        }
    }
}
