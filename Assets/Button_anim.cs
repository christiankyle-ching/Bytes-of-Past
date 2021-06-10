using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class Button_anim : MonoBehaviour
{
    public Animator anim;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonTouch()
    {
        GetComponent<Animation>().Play("btn_touch");
        
    }
    public void ButtonDrag()
    {
        GetComponent<Animation>().Play("btn_touch_end");
    }

    


}
