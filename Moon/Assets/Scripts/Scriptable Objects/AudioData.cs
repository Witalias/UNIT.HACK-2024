using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable Objects/Audio Data", order = 1)]
public class AudioData : ScriptableObject
{
    [SerializeField] private AudioType _type;
    [SerializeField] private AudioClip[] _clips;

    public AudioType Type => _type;
    public AudioClip[] Clips => _clips;
}
