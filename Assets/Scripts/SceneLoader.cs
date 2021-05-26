using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    
    
    public void onClick()
    {
   
         SceneManager.LoadScene("Main Menu");

        
    }

    public  void LoadSoloPlay()
    {
        SceneManager.LoadScene("Solo Play");
    }
    public void LoadMultiPlay()
    {
        SceneManager.LoadScene("Multi Play");

    }
    public void LoadPostAssessment()
    {
        SceneManager.LoadScene("Post Assessment");

    }


}
