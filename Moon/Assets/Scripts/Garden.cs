using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Garden : MonoBehaviour
{
    private const int MAX_GROWTH_STEPS = 4;

    [SerializeField] private Interactable _interactable;
    [SerializeField] private Transform _plantPoint;
    [SerializeField] private Transform _topPoint;
    [SerializeField] private GameObject _cupol;
    [SerializeField] private PlantData[] _plantData;

    private GrowthStage _stage = GrowthStage.Empty;
    private PlantRequirenment _requirenment = PlantRequirenment.No;
    private int _growthStep = 1;
    private ItemType _currentPlantSeed;
    private GameObject _currentPlantObj;
    private float _targetPlantScale;
    private Dictionary<ItemType, PlantData> _plantsInfo;
    private bool _plantInfoIsShown;

    private readonly ItemType[] _seeds = new[]
    {
        ItemType.Plant1Seed,
        ItemType.Plant2Seed,
        ItemType.Plant3Seed
    };

    public Vector3 TopPoint => _topPoint.position;

    public static event Action OnInteract;
    public static event Func<bool> PlayerIsAim;

    private void Awake()
    {
        _plantsInfo = _plantData.ToDictionary(data => data.SeedType);
    }

    private void Start()
    {
        _interactable.SubscribeOnMouseIsOver(MouseIsOver);
        _cupol.SetActive(false);
    }

    private void Update()
    {
        if (_interactable.MouseIsOver)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Plant();
                Harvest();
                ApplyCurrentItem();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Uproot();
            }
            var playerIsAim = PlayerIsAim?.Invoke();
            if (playerIsAim.GetValueOrDefault())
            {
                ShowPlantInfo();
            }
            else
            {
                HidePlantInfo();
            }
        }
        else
        {
            HidePlantInfo();
        }
    }

    private void ShowPlantInfo()
    {
        if (!_plantInfoIsShown)
        {
            _plantInfoIsShown = true;
            var title = "Грядка";
            var properties = GetProperties();
            var propertiesText = $"Состояние: {properties.State}\n";
            if (_stage is not GrowthStage.Empty)
            {
                title = GameData.Instance.GetItemData(_plantsInfo[_currentPlantSeed].PlantType).Name;
                if (properties.Requirenment != "")
                    propertiesText += $"Требуется: {properties.Requirenment}\n";
                propertiesText += "Прогресс роста:";
            }
            UiManager.Instance.ShowPlantInfo(title, propertiesText);
        }
        if (_stage is GrowthStage.Empty)
        {
            UiManager.Instance.RefreshPlantInfo(transform.position);
        }
        else
        {
            UiManager.Instance.RefreshPlantInfo(transform.position, _currentPlantObj.transform.localScale.x, _targetPlantScale);
        }
    }

    private void HidePlantInfo()
    {
        if (_plantInfoIsShown)
        {
            _plantInfoIsShown = false;
            UiManager.Instance.HidePlantInfo();
        }
    }

    private void MouseIsOver(bool value)
    {
        if (value)
        {
            ShowHotkeys();
        }
        else
        {
            UiManager.Instance.HideHotkeys();
        }
    }

    private void Plant()
    {
        var selectedItem = Inventory.Instance.SelectedItem;
        if (_stage is GrowthStage.Empty && Inventory.Instance.IsSelect(_seeds) && Inventory.Instance.TryPeekSelected())
        {
            _growthStep = 0;
            _currentPlantSeed = selectedItem.Value;
            _currentPlantObj = Instantiate(_plantsInfo[_currentPlantSeed].Prefab, _plantPoint.position, Quaternion.identity, transform);
            _stage = GrowthStage.Growth;
            _targetPlantScale = _currentPlantObj.transform.localScale.x;
            AudioManager.Instanse.Play(AudioType.Plant);
            ProcessGrowth();
            OnStageChanged();
            OnInteract?.Invoke();
            _cupol.SetActive(true);
        }
    }

    private void ProcessGrowth()
    {
        Action onCompleteStep = null;
        switch (_growthStep)
        {
            case 1:
                _currentPlantObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                onCompleteStep = () => SetRequirenment(PlantRequirenment.Water);
                break;
            case 2:
                onCompleteStep = () => SetRequirenment(PlantRequirenment.Fertilizer);
                break;
            case 3:
                onCompleteStep = () => SetRequirenment(PlantRequirenment.Water);
                break;
            case 4:
                onCompleteStep = DoHarvestStage;
                break;
        }
        _currentPlantObj.transform.DOScale((_targetPlantScale / MAX_GROWTH_STEPS) * _growthStep, _plantsInfo[_currentPlantSeed].GrowthDuration / MAX_GROWTH_STEPS)
            .OnComplete(() => onCompleteStep?.Invoke());
    }

    private void DoHarvestStage()
    {
        if (_stage is GrowthStage.Growth)
        {
            _stage = GrowthStage.Harvest;
            _interactable.Enabled = true;
            OnStageChanged();
        }
    }

    private void Harvest()
    {
        if (_stage is GrowthStage.Harvest)
        {
            Inventory.Instance.Add(_plantsInfo[_currentPlantSeed].PlantType);
            Inventory.Instance.Add(_currentPlantSeed);
            Inventory.Instance.Add(_currentPlantSeed);
            _stage = GrowthStage.Empty;
            Destroy(_currentPlantObj);
            OnInteract?.Invoke();
            _cupol.SetActive(false);
        }
    }

    private void Uproot()
    {
        if (_stage is not GrowthStage.Empty)
        {
            _stage = GrowthStage.Empty;
            _requirenment = PlantRequirenment.No;
            Inventory.Instance.Add(_currentPlantSeed);
            AudioManager.Instanse.Play(AudioType.Uproot);
            OnStageChanged();
            Destroy(_currentPlantObj);
            OnInteract?.Invoke();
            _cupol.SetActive(false);
        }
    }

    private void ApplyCurrentItem()
    {
        if (_stage == GrowthStage.Require)
        {
            if ((_requirenment is PlantRequirenment.Water && Inventory.Instance.TryPeek(ItemType.Water)) ||
                _requirenment is PlantRequirenment.Fertilizer && Inventory.Instance.TryPeek(ItemType.Fertilizer))
            {
                _stage = GrowthStage.Growth;
                _requirenment = PlantRequirenment.No;
                _growthStep++;
                if (_requirenment is PlantRequirenment.Water)
                    AudioManager.Instanse.Play(AudioType.Water);
                else if (_requirenment is PlantRequirenment.Fertilizer)
                    AudioManager.Instanse.Play(AudioType.Plant);
                OnStageChanged();
                ProcessGrowth();
                OnInteract?.Invoke();
            }
        }
    }

    private void SetRequirenment(PlantRequirenment requirement)
    {
        if (_stage == GrowthStage.Growth)
        {
            _stage = GrowthStage.Require;
            _requirenment = requirement;
            OnStageChanged();
        }
    }

    private void ShowHotkeys()
    {
        switch (_stage)
        {
            case GrowthStage.Empty:
                if (Inventory.Instance.IsSelect(_seeds))
                    UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "Посадить" });
                break;
            case GrowthStage.Growth:
            case GrowthStage.Require:
                UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "R", Description = "Выкорчевать" });
                switch (_requirenment)
                {
                    case PlantRequirenment.Water:
                        if (Inventory.Instance.IsExist(ItemType.Water))
                            UiManager.Instance.AddHotkey(new Hotkey.Data { Key = "E", Description = "Напоить" });
                        break;
                    case PlantRequirenment.Fertilizer:
                        if (Inventory.Instance.IsExist(ItemType.Fertilizer))
                            UiManager.Instance.AddHotkey(new Hotkey.Data { Key = "E", Description = "Удобрить" });
                        break;
                }
                break;
            case GrowthStage.Harvest:
                UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "Собрать" },
                    new Hotkey.Data { Key = "R", Description = "Выкорчевать" });
                break;
        }
    }

    private void OnStageChanged()
    {
        if (_interactable.MouseIsOver)
        {
            ShowHotkeys();
            var playerIsAim = PlayerIsAim?.Invoke();
            if (playerIsAim.GetValueOrDefault())
            {
                _plantInfoIsShown = false;
                ShowPlantInfo();
            }
        }
    }

    private Properties GetProperties()
    {
        var properties = new Properties();
        switch (_stage)
        {
            case GrowthStage.Empty: properties.State = "<color=#B5B5B5>пусто</color>"; break;
            case GrowthStage.Growth: properties.State = "<color=green>растёт</color>"; break;
            case GrowthStage.Harvest: properties.State = "<color=green>готово</color>"; break;
            case GrowthStage.Require: properties.State = "<color=red>не растёт</color>"; break;
        }
        switch (_requirenment)
        {
            case PlantRequirenment.No: properties.Requirenment = ""; break;
            case PlantRequirenment.Water: properties.Requirenment = "<color=red>вода</color>"; break;
            case PlantRequirenment.Fertilizer: properties.Requirenment = "<color=red>удобрение</color>"; break;
        }
        return properties;
    }

    private struct Properties
    {
        public string State { get; set; }
        public string Requirenment { get; set; }
    }
}

public enum GrowthStage
{
    Empty,
    Growth,
    Harvest,
    Require
}

public enum PlantRequirenment
{
    No,
    Water,
    Fertilizer
}