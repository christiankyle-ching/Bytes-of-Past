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
            GameObject.FindWithTag("Static Data").GetComponent<StaticData>();
    }

    public void OnSelectSinglePlayer()
    {
        staticData.SelectedGameMode = GAMEMODE.SinglePlayer;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectMultiplayer()
    {
        staticData.SelectedGameMode = GAMEMODE.Multiplayer;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPreAssessment()
    {
        staticData.SelectedGameMode = GAMEMODE.PreAssessment;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPostAssessment()
    {
        staticData.SelectedGameMode = GAMEMODE.PostAssessment;
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }
}
