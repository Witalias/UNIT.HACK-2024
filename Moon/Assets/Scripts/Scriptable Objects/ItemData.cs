using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [SerializeField] private ItemType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _name;
    [SerializeField] private string _description;

    public ItemType Type => _type;
    public Sprite Sprite => _sprite;
    public string Name => _name;
    public string Description => _description;
}
