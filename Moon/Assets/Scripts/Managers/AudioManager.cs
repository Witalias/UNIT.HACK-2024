using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instanse { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Sounds")]
    [SerializeField] private AudioData[] _audioData;

    private Dictionary<AudioType, AudioClip[]> _sounds;

    private void Awake()
    {
        Instanse = this;
        _sounds = _audioData.ToDictionary(audio => audio.Type, audio => audio.Clips);
    }

    private void Update()
    {
        if (musicAudioSource != null && !musicAudioSource.isPlaying)
            PlayMusic(AudioType.Music);
    }

    public void Play(AudioType sound, AudioSource source = null, bool singleSound = false, float volume = 1.0f)
    {
        if (source == null)
            source = audioSource;

        if (_sounds[sound].Length == 0)
        {
            Debug.LogWarning("There is no audio clip " + sound.ToString());
            return;
        }
        source.volume = volume;

        if (singleSound)
            source.Stop();
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayOneStream(AudioType sound, AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.PlayOneShot(GetRandomClip(sound));
    }

    public void PlayLoop(AudioType sound, AudioSource source = null)
    {
        if (source == null)
            source = loopAudioSource;
        else
            source.loop = true;

        source.clip = GetRandomClip(sound);
        source.Play();
    }

    public void PlayMusic(AudioType sound)
    {
        musicAudioSource.clip = GetRandomClip(sound);
        musicAudioSource.Play();
    }

    public void StopMusic() => musicAudioSource.Stop();

    public void StopLoop(AudioSource source = null)
    {
        if (source == null)
            source = loopAudioSource;
        else
            source.loop = false;

        source.Stop();
    }

    private AudioClip GetRandomClip(AudioType sound) => _sounds[sound][Random.Range(0, _sounds[sound].Length)];
}
