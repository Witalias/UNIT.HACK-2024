using UnityEngine;

[CreateAssetMenu(fileName = "Phase", menuName = "Scriptable Objects/Day Cycle Phase", order = 1)]
public class DayCyclePhaseInfo : ScriptableObject
{
    [SerializeField] private DayCycleType _type;
    [SerializeField] private Color _sunColor;
    [SerializeField] private float _sunIntensity;
    [SerializeField] private float _phaseDuration;
    [SerializeField] private float _transitionDuration;
    [SerializeField] private bool _skipPhase;
    [SerializeField] private AudioType _embientAudio;

    public DayCycleType Type => _type;
    public Color SunColor => _sunColor;
    public float SunIntensity => _sunIntensity;
    public float PhaseDuration => _phaseDuration;
    public float TransitionDuration => _transitionDuration;
    public bool SkipPhase => _skipPhase;
    public AudioType EmbientAudio => _embientAudio;
}
