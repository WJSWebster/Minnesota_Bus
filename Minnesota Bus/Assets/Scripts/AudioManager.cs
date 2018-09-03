using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    private Sound music;
    private Sound casinoAmbient;

    //public AudioMixerGroup mixerGroup;
    
    // used for sliders
    [Range(0f, 1f)]
    private float musicVolumePercentage = 1f;
    [Range(0f, 1f)]
    private float soundEffectVolumePercentage = 1f;

    private float tempMusicVolume;
    private float tempSoundEffectVolume;

    public static AudioManager instance;
    public Sound[] sounds;
    public List<Sound> soundEffects = new List<Sound>();
    
    public float MusicVolumPercentage
    {
        get
        {
            return musicVolumePercentage;
        }
        set
        {
            music.source.volume = music.volume * value; //* (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));

            // if user presses 'ok':
            //musicVolumePercentage = value;

            // if user presses 'cancel':
            //music.source.volume = music.volume * musicVolumePercentage;
        }
    }
    public float SoundEffectVolumPercentage
    {
        get
        {
            return soundEffectVolumePercentage;
        }
        set
        {

            // if user presses 'ok':
            soundEffectVolumePercentage = value;

            // if user presses 'cancel':
            //foreach (Sound soundEffect in soundEffects)
            //{
            //    soundEffect.source.volume = soundEffect.volume * soundEffectVolumePercentage; //* (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            //}
        }
    }

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
	}

    private void Start()
    {
        Play("Music", true);
        Play("CasinoAmbient", false);
    }

    public void Play(string name, bool isMusic = false)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // catches if name not valid
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        float volumePercentage;

        if (isMusic)
        {
            volumePercentage = musicVolumePercentage;
        }
        else // is sound effect
        {
            volumePercentage = soundEffectVolumePercentage;
        }

        s.source.volume = s.volume * volumePercentage; //* (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        // else, sound found:
        //Debug.Log(s.name + ": " + s.source + ", " + s.clip);
        s.source.Play();


        //store music:
        if (isMusic)
            music = s;
        else
        {
            soundEffects.Add(s);
        }
    }

    private void Update()
    {
        foreach(Sound soundEffect in soundEffects)
        {
            if (!soundEffect.source.isPlaying)
            {
                Debug.Log("soundEffect " + soundEffect.name + " is no longer playing, so remove from soundEffects!");
                soundEffects.Remove(soundEffect);
            }
        }
    }

    private void ChangeMusicVolume(Slider slider)
    {
        music.source.volume = music.volume * slider.value; //* (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        
    }

    private void ChangeSoundEffectsVolume(Slider slider)
    {

        foreach (Sound soundEffect in soundEffects)
        {
            soundEffect.source.volume = soundEffect.volume * slider.value; //* (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        }
    }
}
