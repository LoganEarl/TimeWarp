using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

// Call this to play audio >> FindObjectOfType<AudioManager>().PlaySFX/PlayBGM/PlayVoice("enter clipName");

public class AudioManager : MonoBehaviour {

    private static AudioManager instance;

    private static readonly float BGM_DEFAULT_VOLUME = .1f;
    private static readonly float SFX_DEFAULT_VOLUME = .1f;
    private static readonly float VOICE_DEFAULT_VOLUME = .5f;

    [SerializeField]
    private GameObject OptionsMenu;
    [SerializeField]
    private Sound[] bgm, sfx, voice;
    [SerializeField]
    private Slider bgmVolumeSlider, sfxVolumeSlider, voiceVolumeSlider;
    [SerializeField]
    private Toggle friendlyFireToggle;

    private bool voiceClipCurrentlyPlaying = false;

    void Awake()
    {
        CheckInstance();
    }

    void Start()
    {
        LoadAllAudioSources(bgm);
        LoadAllAudioSources(sfx);
        LoadAllAudioSources(voice);
        PlayBGM("Theme");
    }
    
    private void CheckInstance()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public bool GetFriendlyFire()
    {
        return friendlyFireToggle.isOn;
    }

    private void LoadAllAudioSources(Sound[] type)
    {
        foreach (Sound s in type)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            if (type == bgm)
            {
                s.source.volume = BGM_DEFAULT_VOLUME;
                bgmVolumeSlider.value = BGM_DEFAULT_VOLUME;
                s.source.loop = true;
            }
            else if (type == sfx)
            {
                s.source.volume = SFX_DEFAULT_VOLUME;
                sfxVolumeSlider.value = SFX_DEFAULT_VOLUME;
            }
            else
            {
                s.source.volume = VOICE_DEFAULT_VOLUME;
                voiceVolumeSlider.value = VOICE_DEFAULT_VOLUME;
            }

            //s.source.loop = s.loop;
        }
    }

    public static AudioManager GetInstance()
    {
        return instance;
    }

    public GameObject GetOptionsMenu()
    {
        return OptionsMenu;
    }

    public void PlaySFX (string clipName)
    {
        Play(sfx, clipName);
    }

    public void PlayBGM (string clipName)
    {
        Play(bgm, clipName);
    }

    public void PlayVoice (string clipName)
    {
        Play(voice, clipName);
    }

    public float GetClipLength(string clipName)
    {
        Sound tempSound = Array.Find(voice, sound => sound.name == clipName);
        return tempSound.clip.length;
    }

    private void Play(Sound[] type, string clipName)
    {
        Sound soundClip = Array.Find(type, sound => sound.name == clipName);
        if (soundClip == null)
            return;

        if (type == voice && !voiceClipCurrentlyPlaying && !clipName.StartsWith("Announcer"))
            StartCoroutine(PlayVoice(soundClip));
        else if(type == sfx || type == bgm || clipName.StartsWith("Announcer"))
            soundClip.source.Play();
    }

    private IEnumerator PlayVoice(Sound soundClip)
    {
        voiceClipCurrentlyPlaying = true;
        soundClip.source.Play();
        yield return new WaitWhile(() => soundClip.source.isPlaying);
        voiceClipCurrentlyPlaying = false;
    }

    public void UpdateVolume()
    {
        foreach (Sound s in bgm)
        {
            if(s.source != null)
                s.source.volume = bgmVolumeSlider.value;
        }

        foreach (Sound s in sfx)
        {
            if (s.source != null)
                s.source.volume = sfxVolumeSlider.value;
        }

        foreach (Sound s in voice)
        {
            if (s.source != null)
                s.source.volume = voiceVolumeSlider.value;
        }
    }
}
