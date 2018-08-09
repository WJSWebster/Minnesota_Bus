using UnityEngine;
using UnityEngine.Audio;

[System.Serializable] // because is a custom class
public class Sound {
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 1f)]
    public float volumeVariance = .1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float pitchVariance = .5f;

    public bool loop;

    // a value that we populate in AudioManager, so hide from inspector
    [HideInInspector]
    public AudioSource source;
}
