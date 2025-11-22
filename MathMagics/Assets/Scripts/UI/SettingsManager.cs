using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioMixerGroup musicAMG;
    public AudioMixerGroup sfxAMG;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
        
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance.HandleVolumeSliderChanged(musicSlider.value, musicAMG);
    }

    public void ChangeSFXVolume()
    {
        AudioManager.Instance.HandleVolumeSliderChanged(sfxSlider.value, sfxAMG);
    }
}
