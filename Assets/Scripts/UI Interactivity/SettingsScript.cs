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

    public GameObject settingsCanvas;
    public GameObject switchBtn;
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public Button btnUpdate;
    public AppUpdater updater;

    void Awake()
    {
        settingsCanvas.SetActive(false);
        btnUpdate.GetComponentInChildren<TextMeshProUGUI>().text = $"Check for Updates (Current: v{Application.version})";

        // Load last saved settings
        switchState = PlayerPrefs.GetInt(SoundManager.SFXPREFKEY, 1);
        if (switchState < 0) OnSFXSwitchButtonClicked();

        volumeSlider.value = SoundManager.Instance.GetBGMVolume(); // Just set the slider, let the SoundManager apply current volume
        volumeSlider.onValueChanged.AddListener(SetVolume);

        int existingQualityLevel = PlayerPrefs.GetInt("Settings_Quality", 1);
        QualitySettings.SetQualityLevel(existingQualityLevel);
        qualityDropdown.value = existingQualityLevel;
    }

    public void OnSFXSwitchButtonClicked()
    {
        switchBtn
            .transform
            .DOLocalMoveX(-switchBtn.transform.localPosition.x, 0.2f);
        switchState = Math.Sign(-switchBtn.transform.localPosition.x);

        bool enabled = switchState > 0;

        switchBtn.GetComponent<Image>().color =
            !enabled
                ? Color.HSVToRGB(0f, 0f, 0.5f)
                : Color.HSVToRGB(0f, 0f, 1f);

        SoundManager.Instance.SetSFXEnabled(enabled);
        SoundManager.Instance.PlayClickedSFX();
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("Settings_Quality", qualityIndex);
    }

    public void SetVolume(float value)
    {
        volumeSlider.value = value;
        SoundManager.Instance.SetBGMVolume(value);
    }

    public void ForcedCheckUpdate()
    {
        updater.CheckUpdates(true);
        settingsCanvas.SetActive(false);
    }
}
