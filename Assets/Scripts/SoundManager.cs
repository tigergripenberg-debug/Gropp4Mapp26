using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioClip[] wowSounds;
    private AudioSource soundManager;

    private void Start()
    {
        Instance = this;
        soundManager = GetComponent<AudioSource>();
    }

    public void PlayWowSound()
    {
        soundManager.PlayOneShot(wowSounds[Random.Range(0, wowSounds.Length)]);
        Debug.Log("Playing sound inside soundmanager");
    }
}
