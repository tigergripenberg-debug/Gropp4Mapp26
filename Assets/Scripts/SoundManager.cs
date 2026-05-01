using System;
using System.Collections;
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
    [SerializeField] private AudioClip music, comboMusic;
    [SerializeField] private AudioSource sfxSoundManager, musicManager, comboMusicManager;
    [SerializeField] private Slider sfxVolumeSlider, musicVolumeSlider;
    private float fadeDuration = 1f;
    private bool isInComboState = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        musicManager.Play();
    }

    public void StartComboMusic()
    {
        StopAllCoroutines();
        StartCoroutine(CrossFade(musicManager, comboMusicManager));
    }

    public void ExitComboMusic()
    {
        StopAllCoroutines();
        StartCoroutine(CrossFade(comboMusicManager, musicManager));
    }
    
    IEnumerator CrossFade(AudioSource from, AudioSource to)
    {
        float time = 0f;
        if (!to.isPlaying)
        {
            to.Play();
        }
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            from.volume = Mathf.Lerp(1f, 0f, t);
            to.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        from.volume = 0f;
        to.volume = 1f;
        from.Stop();
    }
    

    public void SetMusicVolume(float volume)
    {
        musicManager.volume = musicVolumeSlider.value;
        comboMusicManager.volume = musicVolumeSlider.value;
    }

    public void SetMusicMute(bool mute)
    {
        musicManager.mute = mute;
        comboMusicManager.mute = mute;
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
        Score.OnComboChanged += HandleCombo;
    }

 

    private void OnDisable()
    {
        Score.OnScoreChange -= PlayScoreSound;
        Block.OnBlockPlacement -= PlayPlacementSound;
        GridManager.OnBlockClearedPlayPop -= PlayPop;
        Score.OnComboChanged -= HandleCombo;
    }   
    private void HandleCombo(int combo)
    {
        if (combo >= 2 && !isInComboState)
        {
            isInComboState = true;
            StopAllCoroutines();
            StartCoroutine(CrossFade(musicManager, comboMusicManager));
        }
        else if (combo == 0 && isInComboState)
        {
            isInComboState = false;
            StopAllCoroutines();
            StartCoroutine(CrossFade(comboMusicManager, musicManager));
        }
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