using System;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreEventType
{
    Small,
    Medium,
    Big,
    Jackpot
}

public enum SFXSounds
{
    pop_sound,
    placement_sound
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioClip[] wowSounds;
    [SerializeField] private AudioClip[] music;
    private AudioSource sfxSoundManager, musicSoundManager;
    [SerializeField] private Slider sfxVolumeSlider, musicVolumeSlider;

    private void Awake()
    {
        Instance = this;
        sfxSoundManager = GetComponent<AudioSource>();
        musicSoundManager = GetComponent<AudioSource>();
    }

    private void Start()
    {
        musicSoundManager.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicSoundManager.volume = musicVolumeSlider.value;
    }

    public void SetMusicMute(bool mute)
    {
        musicSoundManager.mute = mute;
    }
    public void SetSFXVolume(float volume)
    {
        sfxSoundManager.volume = sfxVolumeSlider.value;
        PlayerPrefs.SetFloat("volume", volume);
    }
    public void SetSFXMute(bool mute)
    {
        sfxSoundManager.mute = mute;
    }
    public void PlayPop(SFXSounds soundType)
    {
        Play("pop_sound");
    }

    private void OnEnable()
    {
        Score.OnScoreChange += PlayScoreSound;
        Block.OnBlockPlacement += PlayPlacementSound;
        GridManager.OnBlockClearedPlayPop += PlayPop;
    }
    
    private void OnDisable()
    {
        Score.OnScoreChange -= PlayScoreSound;
        Block.OnBlockPlacement -= PlayPlacementSound;
        GridManager.OnBlockClearedPlayPop -= PlayPop;
    }
    private void PlayPlacementSound(SFXSounds soundType)
    {
        Play("placement_sound");
    }
    private void PlayScoreSound(ScoreEventType type)
    {
        switch (type)
        {
            case ScoreEventType.Small:
                Play("small_sound");
                break;
            case ScoreEventType.Medium:
                Play("medium_sound");
                break;
            case ScoreEventType.Big:
                Play("big_sound");
                break;
            case ScoreEventType.Jackpot:
                Play("jackpot_sound");
                break;
        }
    }
    

    void Play(string clipName)
    {
        AudioClip clip = GetClipByName(clipName);
        if (clip != null)
        {
            sfxSoundManager.PlayOneShot(clip);
        }
    }

    AudioClip GetClipByName(string clipName)
    {
        foreach (AudioClip clip in wowSounds)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        Debug.LogWarning("Sound " + clipName + " not found");
        return null;
    }
}