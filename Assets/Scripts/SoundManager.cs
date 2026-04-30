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
    private AudioSource soundManager;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        Instance = this;
        soundManager = GetComponent<AudioSource>();
    }
    

    public void SetVolume(float volume)
    {
        soundManager.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volume);
    }
    public void SetMute(bool mute)
    {
        soundManager.mute = mute;
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
            soundManager.PlayOneShot(clip);
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