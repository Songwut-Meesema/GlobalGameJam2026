using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource sfxSource;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound(SoundData sound)
    {
        sfxSource.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
        sfxSource.PlayOneShot(sound.clip, sound.volume);
    }
}
