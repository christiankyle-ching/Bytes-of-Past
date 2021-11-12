using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    private Animator _animationController;

    private void Start()
    {
        _animationController = GetComponent<Animator>();
    }

    public void Discard()
    {
        // Play Animation
        _animationController.Play("enlarge_fade");

        transform.SetParent(transform.parent.parent); // Move parent one level up to handle spam events
        Destroy(this.gameObject, 1f);
    }
}
