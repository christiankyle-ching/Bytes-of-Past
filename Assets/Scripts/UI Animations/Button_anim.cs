using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class Button_anim : MonoBehaviour
{
    public Animator anim;

    public void ButtonTouch()
    {
        try
        {
            GetComponent<Animation>().Play("btn_touch");
        }
        catch (MissingComponentException)
        {
            Debug.LogError("Animation 'btn_touch' is missing");
        }
    }

    public void ButtonDrag()
    {
        try
        {
            GetComponent<Animation>().Play("btn_touch_end");
        }
        catch (MissingComponentException)
        {
            Debug.LogError("Animation 'btn_touch_end' is missing");
        }
    }
}
