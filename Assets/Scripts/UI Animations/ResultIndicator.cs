using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class ResultIndicator : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    public AudioClip correctSFX, wrongSFX;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowCorrect()
    {
        anim.SetTrigger("Correct");
        audioSource.clip = correctSFX;
        audioSource.Play();
    }

    public void ShowWrong()
    {
        anim.SetTrigger("Wrong");
        audioSource.clip = wrongSFX;
        audioSource.Play();
    }
}
