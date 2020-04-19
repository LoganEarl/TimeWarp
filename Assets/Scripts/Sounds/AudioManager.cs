using System;
using UnityEngine;
using UnityEngine.UI;

// Call this to play audio >> FindObjectOfType<AudioManager>().PlaySFX/PlayBGM/PlayPlayer("clipName");

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BGMPref = "BGMPref";
    private static readonly string SFXPref = "SFXPref";
    private static readonly string PlayerPref = "PlayerPref";
    private int firstPlayInt;
    [SerializeField]
    private Sound[] bgm, sfx, player;
    [SerializeField]
    private Slider bgmVolumeSlider, sfxVolumeSlider, playerVolumeSlider;
    private float bgmFloat, sfxFloat, playerFloat;

    void Start()
    {
        checkInstance();
        LoadAllAudioSources(bgm);
        LoadAllAudioSources(sfx);
        LoadAllAudioSources(player);
        //LoadAudioSettings();
        PlayBGM("Theme");
    }
    
    public void checkInstance()
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

    private void LoadAllAudioSources(Sound[] type)
    {
        foreach (Sound s in type)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            if (type == bgm)
                s.source.volume = bgmVolumeSlider.value;
            else if (type == sfx)
                s.source.volume = sfxVolumeSlider.value;
            else
                s.source.volume = playerVolumeSlider.value;
            s.source.loop = s.loop;
        }
    }

    private void LoadAudioSettings()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if (firstPlayInt == 0)
        {
            bgmFloat = .1f;
            sfxFloat = .5f;
            playerFloat = .3f;
            bgmVolumeSlider.value = bgmFloat;
            sfxVolumeSlider.value = sfxFloat;
            playerVolumeSlider.value = playerFloat;
            PlayerPrefs.SetFloat(BGMPref, bgmFloat);
            PlayerPrefs.SetFloat(SFXPref, sfxFloat);
            PlayerPrefs.SetFloat(PlayerPref, playerFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }

        else
        {
            bgmFloat = PlayerPrefs.GetFloat(BGMPref);
            bgmVolumeSlider.value = bgmFloat;
            sfxFloat = PlayerPrefs.GetFloat(SFXPref);
            sfxVolumeSlider.value = sfxFloat;
            playerFloat = PlayerPrefs.GetFloat(PlayerPref);
            playerVolumeSlider.value = playerFloat;
        }
    }
    
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat(BGMPref, bgmVolumeSlider.value);
        PlayerPrefs.SetFloat(SFXPref, sfxVolumeSlider.value);
        PlayerPrefs.SetFloat(PlayerPref, playerVolumeSlider.value);
    }
    
    void OnApplicationFocus(bool focus)
    {
        if (!focus)
            SaveAudioSettings();
    }

    public void PlaySFX (string clipName)
    {
        Play(sfx, clipName);
    }

    public void PlayBGM (string clipName)
    {
        Play(bgm, clipName);
    }

    public void PlayPlayer (string clipName)
    {
        Play(player, clipName);
    }

    private void Play(Sound[] type, string clipName)
    {
        Sound s = Array.Find(type, sound => sound.name == clipName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + clipName + " not found!");
            return;
        }
        s.source.Play();
    }

    public void UpdateVolume()
    {
        foreach(Sound s in bgm)
        {
            s.source.volume = bgmVolumeSlider.value;
        }

        foreach (Sound s in sfx)
        {
            s.source.volume = sfxVolumeSlider.value;
        }

        foreach (Sound s in player)
        {
            s.source.volume = playerVolumeSlider.value;
        }

        //SaveAudioSettings();
    }
}
