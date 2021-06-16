using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    private int switchState = 1;

    public GameObject switchBtn;

    public GameObject slider;

    public GameObject dropdown;

    //Audios
    AudioSource bgmAudioSource;

    void Awake()
    {
        // Audio Source for BGM, now a prefab and has a SingleInstance component
        // that maintains a single instance of BGMAudioSource throughout different scenes
        bgmAudioSource =
            GameObject
                .FindGameObjectWithTag("BGMAudioSource")
                .GetComponent<AudioSource>();

        // Load last saved settings
        switchState = PlayerPrefs.GetInt("Settings_EnableSFX", 1);
        if (switchState == -1) OnSwitchButtonClicked();

        float existingVolume = PlayerPrefs.GetFloat("Settings_BGMVolume", 1.0f);
        bgmAudioSource.volume = existingVolume;
        slider.GetComponent<Slider>().value = existingVolume;

        int existingQualityLevel = PlayerPrefs.GetInt("Settings_Quality", 1);
        QualitySettings.SetQualityLevel (existingQualityLevel);
        dropdown.GetComponent<TMP_Dropdown>().value = existingQualityLevel;
    }

    public void OnSwitchButtonClicked()
    {
        switchBtn
            .transform
            .DOLocalMoveX(-switchBtn.transform.localPosition.x, 0.2f);
        switchState = Math.Sign(-switchBtn.transform.localPosition.x);

        switchBtn.GetComponent<Image>().color =
            switchState == -1
                ? Color.HSVToRGB(0f, 0f, 0.5f)
                : Color.HSVToRGB(0f, 0f, 1f);

        PlayerPrefs.SetInt("Settings_EnableSFX", switchState);
    }

    public void setVolume(float volume)
    {
        bgmAudioSource.volume = volume;

        PlayerPrefs.SetFloat("Settings_BGMVolume", volume);
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel (qualityIndex);

        PlayerPrefs.SetInt("Settings_Quality", qualityIndex);
    }
}
