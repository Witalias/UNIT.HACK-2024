using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [SerializeField] private ItemData[] _itemData;
    [SerializeField] private Transform[] _enemyPatrolPoints;

    private Dictionary<ItemType, ItemData> _itemsInfo;

    private void Awake()
    {
        Instance = this;
        _itemsInfo = _itemData.ToDictionary(data => data.Type);
    }

    private void Start()
    {
        //AudioManager.Instanse.PlayMusic(AudioType.Music);
    }

    public ItemData GetItemData(ItemType type) => _itemsInfo[type];

    public Vector3 GetRandomEnemyPatrolPoint() => _enemyPatrolPoints[Random.Range(0, _enemyPatrolPoints.Length)].position;
}
