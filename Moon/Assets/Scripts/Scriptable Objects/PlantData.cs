using UnityEngine;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/Plant Data", order = 1)]
public class PlantData : ScriptableObject
{
    [SerializeField] private ItemType _plantType;
    [SerializeField] private ItemType _seedType;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _growthDuration;

    public ItemType PlantType => _plantType;
    public ItemType SeedType => _seedType;
    public GameObject Prefab => _prefab;
    public float GrowthDuration => _growthDuration;
}
