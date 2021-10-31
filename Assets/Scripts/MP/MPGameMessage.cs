using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MPGameMessageType
{
    CORRECT, WRONG, SA_SKIP, SA_PEEK, SA_DOUBLE, TRADE, NONE
}

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class MPGameMessage : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;

    public TextMeshProUGUI _label;
    public Image _image;

    [Header("Images")]
    public Sprite checkmark;
    public Sprite cross;
    public Sprite trade;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip defaultSFX;
    public AudioClip wrongSFX;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowMessage(string message, MPGameMessageType type = MPGameMessageType.NONE)
    {
        _label.text = message;
        SetImage(type);

        anim.SetTrigger("Show");
    }

    private void SetImage(MPGameMessageType type)
    {
        switch (type)
        {
            case MPGameMessageType.CORRECT:
                _image.sprite = checkmark;
                PlayCorrectSFX();
                break;
            case MPGameMessageType.WRONG:
                _image.sprite = cross;
                PlayWrongSFX();
                break;
            case MPGameMessageType.SA_SKIP:
                _image.sprite = MPCardInfo.GetSpecialActionSprite(SPECIALACTION.SkipTurn);
                PlayDefaultSFX();
                break;
            case MPGameMessageType.SA_PEEK:
                _image.sprite = MPCardInfo.GetSpecialActionSprite(SPECIALACTION.Peek);
                PlayDefaultSFX();
                break;
            case MPGameMessageType.SA_DOUBLE:
                _image.sprite = MPCardInfo.GetSpecialActionSprite(SPECIALACTION.DoubleDraw);
                PlayDefaultSFX();
                break;
            case MPGameMessageType.TRADE:
                _image.sprite = trade;
                PlayDefaultSFX();
                break;
            default:
                _image.sprite = null;
                PlayDefaultSFX();
                break;
        }
    }

    private void PlayCorrectSFX()
    {
        audioSource.clip = correctSFX;
        audioSource.Play();
    }

    private void PlayDefaultSFX()
    {
        audioSource.clip = defaultSFX;
        audioSource.Play();
    }

    private void PlayWrongSFX()
    {
        audioSource.clip = wrongSFX;
        audioSource.Play();
    }
}

