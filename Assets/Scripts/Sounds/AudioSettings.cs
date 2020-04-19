using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    private static readonly string BGMPref = "BGMPref";
    private static readonly string SFXPref = "SFXPref";
    private static readonly string PlayerPref = "PlayerPref";
    
    private Sound[] bgm, sfx, player;
    private float bgmFloat, sfxFloat, playerFloat;

    //public AudioManager audioManager;

    void Awake()
    {
        LoadPreviousAudioSettings();
        UpdateAudioSettings();
    }
    
    private void LoadPreviousAudioSettings()
    {
        bgmFloat = PlayerPrefs.GetFloat(BGMPref);
        sfxFloat = PlayerPrefs.GetFloat(SFXPref);
        playerFloat = PlayerPrefs.GetFloat(PlayerPref);
    }

    public void UpdateAudioSettings()
    {
        foreach (Sound s in bgm)
        {
            s.source.volume = bgmFloat;
        }

        foreach (Sound s in sfx)
        {
            s.source.volume = sfxFloat;
        }

        foreach (Sound s in player)
        {
            s.source.volume = playerFloat;
        }
    }
}
