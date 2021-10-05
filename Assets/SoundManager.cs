using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip clickSFX;

    public AudioSource audioSource;

    public void playClickedSFX()
    {
        audioSource.clip = clickSFX;
        audioSource.Play();
    }
}
