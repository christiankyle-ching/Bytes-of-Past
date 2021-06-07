using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void onClick()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadSoloPlay()
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
