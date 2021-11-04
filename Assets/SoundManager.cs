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
    public AudioSource CardSFXAudioSource; // needed separate SFX Audio Source to play other SFX concurrently
    public AudioSource SFXAudioSource;
    public AudioSource BGMAudioSource;
    public AudioMixerGroup BGMAudioMixer;

    bool isSFXEnabled = true;
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
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        CardSFXAudioSource.playOnAwake = false;
        SFXAudioSource.playOnAwake = false;

        BGMAudioSource.clip = backgroundMusic;
        BGMAudioSource.outputAudioMixerGroup = BGMAudioMixer;
        BGMAudioSource.Play();
    }

    #region ------------------------------ SFX METHODS ------------------------------

    public void PlayClickedSFX() => PlaySFX(clickSFX);

    public void PlayCorrectSFX() => PlaySFX(correctSFX);

    public void PlayWrongSFX() => PlaySFX(wrongSFX);

    public void PlayDefaultSFX() => PlaySFX(defaultSFX);

    public void PlayTimerTickSFX() => PlaySFX(timerTickSFX);

    // ------------------------------ CARD SFX ------------------------------

    public void PlayMultipleDrawSFX() => PlayCardSFX(multipleDrawSFX);

    public void PlayDrawSFX() => PlayCardSFX(drawSFX);

    public void PlayDiscardSFX() => PlayCardSFX(discardSFX);

    #endregion

    #region ------------------------------ UTILS ------------------------------

    private void PlaySFX(AudioClip _clip)
    {
        if (isSFXEnabled)
        {
            SFXAudioSource.clip = _clip;
            SFXAudioSource.Play();
        }
    }

    private void PlayCardSFX(AudioClip _clip)
    {
        if (isSFXEnabled)
        {
            CardSFXAudioSource.clip = _clip;
            CardSFXAudioSource.Play();
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
        Debug.Log(sliderValue);
        BGMAudioMixer.audioMixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(BGMVOLPREFKEY, sliderValue);
    }

    #endregion
}
