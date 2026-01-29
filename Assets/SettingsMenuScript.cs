using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuScript : MonoBehaviour
{
    public Slider MasterVolume, MusicVolume, SFXVolume;
    public AudioMixer AudioMixer;

    public void ChangeMasterVolume()

    {
        AudioMixer.SetFloat("MasterVolume", MasterVolume.value);
    }

    public void ChangeMusicVolume()

    {
        AudioMixer.SetFloat("MusicVolume", MasterVolume.value);
    }

    public void ChangeSFXVolume()

    {
        AudioMixer.SetFloat("SFXVolume", MasterVolume.value);
    }


       
    
}
