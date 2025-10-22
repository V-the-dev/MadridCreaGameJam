using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;

public class SettingsManager : MonoBehaviour
{
    public Slider musicVol, sfxVol, ambientVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("Volume_Music", musicVol.value);
    }
    
    public void ChangeSfxVolume()
    {
        mainAudioMixer.SetFloat("Volume_SFX", sfxVol.value);
    }
    
    public void ChangeAmbientVolume()
    {
        mainAudioMixer.SetFloat("Volume_Ambient", ambientVol.value);
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
