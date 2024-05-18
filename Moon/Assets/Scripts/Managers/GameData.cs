using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [SerializeField] private ItemData[] _itemData;

    private Dictionary<ItemType, ItemData> _itemsInfo;

    private void Awake()
    {
        Instance = this;
        _itemsInfo = _itemData.ToDictionary(data => data.Type);
    }

    public ItemData GetItemData(ItemType type) => _itemsInfo[type];
}
