using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class ResultIndicator : MonoBehaviour
{
    private Animator anim;
    public AudioClip correctSFX, wrongSFX;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ShowCorrect()
    {
        anim.SetTrigger("Correct");
        SoundManager.Instance.PlayCorrectSFX();
    }

    public void ShowWrong()
    {
        anim.SetTrigger("Wrong");
        SoundManager.Instance.PlayWrongSFX();
    }
}
