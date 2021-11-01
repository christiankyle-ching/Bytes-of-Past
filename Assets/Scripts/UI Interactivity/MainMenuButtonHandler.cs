using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonHandler : MonoBehaviour
{
    StaticData staticData;

    public SceneLoader sceneLoader;

    public GameObject settingsCanvas;

    void Start()
    {
        staticData = StaticData.Instance;
    }

    public void OnSelectSinglePlayer()
    {
        staticData.SetGameMode(GAMEMODE.SinglePlayer);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectMultiplayer()
    {
        staticData.SetGameMode(GAMEMODE.Multiplayer);
        sceneLoader.GetComponent<SceneLoader>().GoToMultiplayerLobby();
    }

    public void OnSelectPreAssessment()
    {
        staticData.SetGameMode(GAMEMODE.PreAssessment);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPostAssessment()
    {
        staticData.SetGameMode(GAMEMODE.PostAssessment);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectTutorial()
    {
        sceneLoader.GetComponent<SceneLoader>().GoToTutorial();
    }

    public void onSelectGameSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void onSelectCredits()
    {
        sceneLoader.GetComponent<SceneLoader>().GoToCredits();
    }

    public void onClose()
    {
        settingsCanvas.SetActive(false);
    }
}
