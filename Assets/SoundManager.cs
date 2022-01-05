using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip backgroundMusic;
    public AudioClip clickSFX;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;
    public AudioClip defaultSFX;
    public AudioClip timerTickSFX;

    [Header("Cards SFX")]
    public AudioClip multipleDrawSFX;
    public AudioClip drawSFX;
    public AudioClip discardSFX;

    [Header("AudioSources")]
    public AudioSource SFXAudioSource;
    public AudioSource BGMAudioSource;
    public AudioMixerGroup BGMAudioMixer;

    bool isSFXEnabled = false;
    public static readonly string SFXPREFKEY = "SFXEnabled";
    public static readonly string BGMVOLPREFKEY = "BGMVolume";

    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        SFXAudioSource.loop = false;
        SFXAudioSource.playOnAwake = false;

        BGMAudioSource.loop = true;
        BGMAudioSource.clip = backgroundMusic;
        BGMAudioSource.outputAudioMixerGroup = BGMAudioMixer;
        SetBGMVolume(GetBGMVolume());
        BGMAudioSource.Play();
    }

    #region ------------------------------ SFX METHODS ------------------------------

    public void PlayClickedSFX() => PlaySFX(clickSFX);

    public void PlayCorrectSFX() => PlaySFX(correctSFX);

    public void PlayWrongSFX() => PlaySFX(wrongSFX);

    public void PlayDefaultSFX() => PlaySFX(defaultSFX);

    public void PlayTimerTickSFX() => PlaySFX(timerTickSFX);

    // ------------------------------ CARD SFX ------------------------------

    public void PlayMultipleDrawSFX() => PlaySFX(multipleDrawSFX);

    public void PlayDrawSFX() => PlaySFX(drawSFX);

    public void PlayDiscardSFX() => PlaySFX(discardSFX);

    #endregion

    #region ------------------------------ UTILS ------------------------------

    private void PlaySFX(AudioClip _clip)
    {
        if (isSFXEnabled)
        {
            SFXAudioSource.PlayOneShot(_clip);
        }
    }

    public void SetSFXEnabled(bool enabled)
    {
        Debug.Log($"SFX Enabled: {enabled}");
        isSFXEnabled = enabled;
        PlayerPrefs.SetInt(SFXPREFKEY, enabled ? 1 : -1);
    }

    public void SetBGMVolume(float sliderValue)
    {
        BGMAudioMixer.audioMixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(BGMVOLPREFKEY, sliderValue);
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat(BGMVOLPREFKEY, 0.5f);
    }

    #endregion
}
