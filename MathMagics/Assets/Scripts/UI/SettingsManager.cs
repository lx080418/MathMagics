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
    public GameObject settingsMenu;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioMixerGroup musicAMG;
    public AudioMixerGroup sfxAMG;
    public string exposedMusicParamString = "MusicVolume";
    public string exposedSFXParamString = "SFXVolume";
    public bool easyMode = false;
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

        settingsMenu.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance.HandleVolumeSliderChanged(musicSlider.value, exposedMusicParamString);
    }

    public void ChangeSFXVolume()
    {
        AudioManager.Instance.HandleVolumeSliderChanged(sfxSlider.value, exposedSFXParamString);
    }

    public void ToggleMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void OnEasyModeToggled()
    {
        if(easyMode == true)
        {
            easyMode = false;
        }
        else
        {
            easyMode = true;
        }
    }
}
