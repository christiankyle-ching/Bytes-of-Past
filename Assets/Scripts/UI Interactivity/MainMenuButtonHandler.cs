using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonHandler : MonoBehaviour
{
    StaticData staticData;

    public SceneLoader sceneLoader;

    void Awake()
    {
        staticData =
            GameObject.FindWithTag("UI Manager").GetComponent<StaticData>();
    }

    public void OnSelectSinglePlayer()
    {
        Debug.Log("Click");
        staticData.SelectedGameMode = GAMEMODE.SinglePlayer;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectMultiplayer()
    {
        Debug.Log("Click");
        staticData.SelectedGameMode = GAMEMODE.Multiplayer;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPreAssessment()
    {
        Debug.Log("Click");
        staticData.SelectedGameMode = GAMEMODE.PreAssessment;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPostAssessment()
    {
        Debug.Log("Click");
        staticData.SelectedGameMode = GAMEMODE.PostAssessment;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }
}
