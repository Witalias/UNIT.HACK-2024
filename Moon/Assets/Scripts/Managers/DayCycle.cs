using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DayCycle : MonoBehaviour
{
    public static DayCycle Instance { get; private set; }

    [SerializeField] private DayCyclePhaseInfo[] _phases;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Light _sun;

    private Dictionary<DayCycleType, DayCyclePhaseInfo> _phasesDict;
    private Tween _currentTween;

    public static event Action<DayCycleType> GPhaseChanged;

    public DayCycleType CurrentPhase { get; private set; } = DayCycleType.Morning;

    private void Awake()
    {
        Instance = this;
        _phasesDict = _phases.ToDictionary(phase => phase.Type);
    }

    private void Start()
    {
        NextPhase();
    }

    public void RunPhase(DayCycleType type)
    {
        ProcessPhase(_phasesDict[type]);
    }

    public void NextPhase()
    {
        ProcessPhase(GetNextPhase(CurrentPhase));
    }

    private void ProcessPhase(DayCyclePhaseInfo phase)
    {
        CurrentPhase = phase.Type;
        Debug.Log($"Current phase: {phase.Type}");
        if (phase.SkipPhase)
        {
            NextPhase();
            return;
        }
        _currentTween?.Kill();
        _currentTween = DOTween.Sequence()
            .Append(_sun.DOColor(phase.SunColor, phase.TransitionDuration))
            .Insert(0, _sun.DOIntensity(phase.SunIntensity, phase.TransitionDuration))
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GPhaseChanged?.Invoke(phase.Type);
                //AudioManager.Instanse.PlayLoop(phase.EmbientAudio, _audioSource);
                _currentTween = DOVirtual.DelayedCall(phase.PhaseDuration, () => ProcessPhase(GetNextPhase(phase.Type)));
            })
            .Play();
    }

    private DayCyclePhaseInfo GetNextPhase(DayCycleType type)
    {
        return type switch
        {
            DayCycleType.Day => _phasesDict[DayCycleType.Evening],
            DayCycleType.Evening => _phasesDict[DayCycleType.Night],
            DayCycleType.Night => _phasesDict[DayCycleType.Morning],
            DayCycleType.Morning => _phasesDict[DayCycleType.Day],
            _ => null,
        };
    }
}
