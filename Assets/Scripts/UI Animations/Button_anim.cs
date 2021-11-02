using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button_anim : MonoBehaviour
{
    
    public void btnTouch(Button btn)
    {
        btn.transform.localScale = new Vector2(.9f, .9f);
    }

}
