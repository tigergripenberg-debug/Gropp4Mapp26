using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioClip[] wowSounds;
    private AudioSource soundManager;

    private void Awake()
    {
        Instance = this;
        soundManager = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Score.OnScoreChange += PlaySound;
    }

    private void OnDisable()
    {
        Score.OnScoreChange -= PlaySound;
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = GetClipByName(soundName);

        if (clip != null)
        {
            soundManager.PlayOneShot(clip);
        }
    }

    AudioClip GetClipByName(string name)
    {
        foreach (AudioClip clip in wowSounds)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        Debug.LogWarning("Clip not found: " + name);
        return null;
    }
    
    public string GetRandomAudioClip()
    {
        AudioClip clip = wowSounds[Random.Range(0, wowSounds.Length)];
        return clip.name;
    }
}