using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;

public class SettingsManager : MonoBehaviour
{
    public Slider musicVol, sfxVol, ambientVol;
    public AudioMixer mainAudioMixer;

    private const string MUSIC_KEY = "Volume_Music";
    private const string SFX_KEY = "Volume_SFX";
    private const string AMBIENT_KEY = "Volume_Ambient";

    private void Start()
    {
        // Cargar volúmenes guardados o usar valores por defecto
        float musicValue = PlayerPrefs.GetFloat(MUSIC_KEY, 0f);
        float sfxValue = PlayerPrefs.GetFloat(SFX_KEY, 0f);
        float ambientValue = PlayerPrefs.GetFloat(AMBIENT_KEY, 0f);

        // Aplicar al AudioMixer
        mainAudioMixer.SetFloat(MUSIC_KEY, musicValue);
        mainAudioMixer.SetFloat(SFX_KEY, sfxValue);
        mainAudioMixer.SetFloat(AMBIENT_KEY, ambientValue);

        // Actualizar sliders (sin disparar OnValueChanged)
        if (musicVol) 
            musicVol.SetValueWithoutNotify(musicValue);
        
        if (sfxVol) 
            sfxVol.SetValueWithoutNotify(sfxValue);
        
        if (ambientVol) 
            ambientVol.SetValueWithoutNotify(ambientValue);
    }

    public void ChangeMusicVolume()
    {
        float value = musicVol.value;
        mainAudioMixer.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();
        
    }
    
    public void ChangeSfxVolume()
    {
        float value = sfxVol.value;
        mainAudioMixer.SetFloat(SFX_KEY, value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
        PlayerPrefs.Save();
        
    }
    
    public void ChangeAmbientVolume()
    {
        float value = ambientVol.value;
        mainAudioMixer.SetFloat(AMBIENT_KEY, value);
        PlayerPrefs.SetFloat(AMBIENT_KEY, value);
        PlayerPrefs.Save();
        
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
