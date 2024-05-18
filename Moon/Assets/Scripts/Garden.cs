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
    [SerializeField] private PlantData[] _plantData;

    private GrowthStage _stage = GrowthStage.Empty;
    private PlantRequirenment _requirenment = PlantRequirenment.No;
    private int _growthStep = 1;
    private ItemType _currentPlantSeed;
    private GameObject _currentPlantObj;
    private float _targetPlantScale;
    private Dictionary<ItemType, PlantData> _plantsInfo;
    private bool _plantInfoIsShown;

    public static event Action OnInteract;
    public static event Func<bool> PlayerIsAim;

    private void Awake()
    {
        _plantsInfo = _plantData.ToDictionary(data => data.SeedType);
    }

    private void Start()
    {
        _interactable.SubscribeOnMouseIsOver(MouseIsOver);
    }

    private void Update()
    {
        if (_interactable.MouseIsOver)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Plant();
                Harvest();
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
            _currentPlantSeed = ItemType.Plant1Seed;
            var title = "������";
            var properties = GetProperties();
            var propertiesText = $"���������: {properties.State}\n";
            if (_stage is not GrowthStage.Empty)
            {
                title = GameData.Instance.GetItemData(_plantsInfo[_currentPlantSeed].PlantType).Name;
                if (properties.Requirenment != "")
                    propertiesText += $"���������: {properties.Requirenment}\n";
                propertiesText += "�������� �����:";
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
        if (_stage is GrowthStage.Empty)
        {
            _currentPlantSeed = ItemType.Plant1Seed;
            _currentPlantObj = Instantiate(_plantsInfo[_currentPlantSeed].Prefab, _plantPoint.position, Quaternion.identity, transform);
            _stage = GrowthStage.Growth;
            _targetPlantScale = _currentPlantObj.transform.localScale.x;
            ProcessGrowth();
            OnStageChanged();
            OnInteract?.Invoke();
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
            _stage = GrowthStage.Empty;
            Destroy(_currentPlantObj);
            OnInteract?.Invoke();
        }
    }

    private void Uproot()
    {
        if (_stage is not GrowthStage.Empty)
        {
            _stage = GrowthStage.Empty;
            _requirenment = PlantRequirenment.No;
            Inventory.Instance.Add(_currentPlantSeed);
            OnStageChanged();
            Destroy(_currentPlantObj);
            OnInteract?.Invoke();
        }
    }

    private void ApplyCurrentItem()
    {
        if (_stage == GrowthStage.Require)
        {
            _stage = GrowthStage.Growth;
            _growthStep++;
            ProcessGrowth();
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
                UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "��������" });
                break;
            case GrowthStage.Growth:
            case GrowthStage.Require:
                UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "R", Description = "�����������" });
                break;
            case GrowthStage.Harvest:
                UiManager.Instance.ShowHotkeys(new Hotkey.Data { Key = "E", Description = "�������" },
                    new Hotkey.Data { Key = "R", Description = "�����������" });
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
                _plantInfoIsShown = false;
        }
    }

    private Properties GetProperties()
    {
        var properties = new Properties();
        switch (_stage)
        {
            case GrowthStage.Empty: properties.State = "<color=#B5B5B5>�����</color>"; break;
            case GrowthStage.Growth: properties.State = "<color=green>�����</color>"; break;
            case GrowthStage.Harvest: properties.State = "<color=green>������</color>"; break;
            case GrowthStage.Require: properties.State = "<color=red>�� �����</color>"; break;
        }
        switch (_requirenment)
        {
            case PlantRequirenment.No: properties.Requirenment = ""; break;
            case PlantRequirenment.Water: properties.Requirenment = "<color=red>����</color>"; break;
            case PlantRequirenment.Fertilizer: properties.Requirenment = "<color=red>���������</color>"; break;
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