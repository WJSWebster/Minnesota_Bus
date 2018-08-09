using System;

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    //public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    // Use this for initialization
    void Awake () {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            //s.source.outputAudioMixerGroup = mixerGroup;
        }
	}

    private void Start()
    {
        Play("Music");
        Play("CasinoAmbient");
    }

    public void Play(string name)
    {
        // catch if name not a sound
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)  //if not found
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        // else, sound found:
        Debug.Log(s.name + ": " + s.source + ", " + s.clip);
        s.source.Play();
        
    }
}
