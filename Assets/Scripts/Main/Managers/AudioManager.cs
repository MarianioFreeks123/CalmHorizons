using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum SoundType
{
    WALKINGSOUNDS,
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
    [Header("SOUND LIST STRUCTURE")]
    [Tooltip("Lista de sonidos organizados por tipo.")]
    [SerializeField] private SoundList[] soundList;

    [Header("MUSIC LIST STRUCTURE")]
    [Tooltip("Lista de pistas de música organizadas por tipo.")]
    [SerializeField] private MusicList[] musicList;

    [Header("REFERENCES IN SCENE")]
    [Tooltip("AudioSource para reproducir sonidos.")]
    [SerializeField] private AudioSource soundAudioSource;
    [Tooltip("AudioSource para reproducir música.")]
    [SerializeField] private AudioSource musicAudioSource;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);

        else instance = this;
    }

    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        SoundList selectedSound = Array.Find(instance.soundList, s => s.type == sound);

        if (selectedSound.sounds.Length > 0)
        {
            AudioClip clip = selectedSound.sounds[UnityEngine.Random.Range(0, selectedSound.sounds.Length)];
            instance.soundAudioSource.PlayOneShot(clip, selectedSound.volume * volume);
        }
        else Debug.LogWarning("No sounds assigned to " + sound);

    }

    public static void PlayMusic(MusicType music, float volume = 1f, bool loop = true)
    {
        MusicList selectedMusic = Array.Find(instance.musicList, m => m.type == music);

        if (selectedMusic.musicClip != null)
        {
            instance.musicAudioSource.clip = selectedMusic.musicClip;
            instance.musicAudioSource.volume = selectedMusic.volume * volume;
            instance.musicAudioSource.loop = loop;
            instance.musicAudioSource.Play();
        }
        else Debug.LogWarning("No music assigned to " + music);
    }

    public static IEnumerator FadeOutMusic(float fadeTime)
    {
        float startVolume = instance.musicAudioSource.volume;

        while (instance.musicAudioSource.volume > 0)
        {
            instance.musicAudioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        instance.musicAudioSource.Stop();
        instance.musicAudioSource.volume = startVolume;
    }

    public static IEnumerator FadeInMusic(MusicType music, float fadeTime, float volume = 1f, bool loop = true)
    {
        PlayMusic(music, 0, loop); // Inicia la música con volumen cero
        instance.musicAudioSource.volume = 0;

        while (instance.musicAudioSource.volume < volume)
        {
            instance.musicAudioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}

[Serializable]
public struct SoundList
{
    [Tooltip("Nombre del sonido (para referencia en el inspector)")]
    public string name;

    [Tooltip("Tipo de sonido (debe coincidir con el enum SoundType)")]
    public SoundType type;

    [Tooltip("Volumen general del sonido (entre 0 y 1)")]
    [Range(0, 1)] public float volume;

    [Tooltip("Clips de audio asociados al tipo de sonido.")]
    public AudioClip[] sounds;

    [Tooltip("Si se debe reproducir en bucle o no.")]
    public bool loop;

    [Tooltip("Si el sonido se reproduce inmediatamente al cargar la escena.")]
    public bool playOnAwake;
}

[Serializable]
public struct MusicList
{
    [Tooltip("Nombre de la música (para referencia en el inspector)")]
    public string name;

    [Tooltip("Tipo de música (debe coincidir con el enum MusicType)")]
    public MusicType type;

    [Tooltip("Volumen general de la música (entre 0 y 1)")]
    [Range(0, 1)] public float volume;

    [Tooltip("Clip de audio de la música.")]
    public AudioClip musicClip;

    [Tooltip("Si la música se reproduce en bucle o no.")]
    public bool loop;

    [Tooltip("Si la música se reproduce inmediatamente al cargar la escena.")]
    public bool playOnAwake;
}