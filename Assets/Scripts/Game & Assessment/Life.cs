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

        StartCoroutine(DestroyGameObject());
    }

    private IEnumerator DestroyGameObject()
    {
        // Delay to play animation
        yield return new WaitForSeconds(1);

        Destroy(this.gameObject);
    }
}
