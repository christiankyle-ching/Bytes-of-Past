using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject soloPlay;
    public GameObject multiPlay;
    public GameObject PostAssessment;

    public void onButtonHightlight(int buttonIndex)
    {
        
        switch (buttonIndex)
        {
            case 0:
                soloPlay.transform.localScale = new Vector2(1.05f, 1.05f);
                multiPlay.transform.localScale = new Vector2(1, 1);
                PostAssessment.transform.localScale = new Vector2(1, 1);
                break;

            case 1:
                soloPlay.transform.localScale = new Vector2(1, 1);
                multiPlay.transform.localScale = new Vector2(1.05f, 1.05f);
                PostAssessment.transform.localScale = new Vector2(1, 1);
                break;

            case 2:
                soloPlay.transform.localScale = new Vector2(1, 1);
                multiPlay.transform.localScale = new Vector2(1, 1);
                PostAssessment.transform.localScale = new Vector2(1.05f, 1.05f);
                break;
        }
        
    }

   
 
}
