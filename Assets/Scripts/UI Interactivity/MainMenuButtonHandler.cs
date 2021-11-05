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
        
        staticData.SetGameMode(GameMode.SP);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectMultiplayer()
    {
        
        staticData.SetGameMode(GameMode.MP);
        sceneLoader.GetComponent<SceneLoader>().GoToMultiplayerLobby();
    }

    public void OnSelectPreAssessment()
    {
        
        staticData.SetGameMode(GameMode.PRE_TEST);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPostAssessment()
    {
        
        staticData.SetGameMode(GameMode.POST_TEST);
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
