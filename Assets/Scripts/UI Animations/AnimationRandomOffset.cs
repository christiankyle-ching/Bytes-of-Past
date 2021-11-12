using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationRandomOffset : MonoBehaviour
{
    void Start()
    {
        GetComponent<Animator>().SetFloat("Offset", Random.Range(0f, 1f));
    }
}
