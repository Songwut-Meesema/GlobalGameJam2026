using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound")]
public class SoundData : ScriptableObject
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 1f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1f;
}

