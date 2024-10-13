using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    ANCHOR,
    JUMP,
    DEATH,
}

public enum MusicType
{
    MAINMENU,
    FIRSTBIOME,
    SECONDBIOME,
}

public class AudioManager : MonoBehaviour
{
    [Header("SOUNDS")]
    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] musicList;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] AudioSource soundAudioSource;
    [SerializeField] AudioSource musicAudioSource;

    public static AudioManager instance;

    //Create the singleton
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        else instance = this;
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.soundAudioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    public static void PlayMusic(MusicType sound, float volume = 1)
    {
        instance.musicAudioSource.volume = volume;
        instance.musicAudioSource.clip = instance.musicList[(int)sound];
    }
}
