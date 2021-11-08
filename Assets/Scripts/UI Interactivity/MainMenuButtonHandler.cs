using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonHandler : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public GameObject settingsCanvas;

    public void OnSelectSinglePlayer()
    {
        StaticData.Instance.SetGameMode(GameMode.SP);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectMultiplayer()
    {
        StaticData.Instance.SetGameMode(GameMode.MP);
        sceneLoader.GetComponent<SceneLoader>().GoToMultiplayerLobby();
    }

    public void OnSelectPreAssessment()
    {
        
        StaticData.Instance.SetGameMode(GameMode.PRE_TEST);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectPostAssessment()
    {
        
        StaticData.Instance.SetGameMode(GameMode.POST_TEST);
        sceneLoader.GetComponent<SceneLoader>().GoToTopicSelect();
    }

    public void OnSelectTutorial()
    {
        sceneLoader.GetComponent<SceneLoader>().GoToTutorial();
    }

    public void onSelectGameSettings()
    {
        SoundManager.Instance.PlayClickedSFX();
        settingsCanvas.SetActive(true);
    }

    public void onSelectCredits()
    {
        sceneLoader.GetComponent<SceneLoader>().GoToCredits();
    }

    public void onClose()
    {
        SoundManager.Instance.PlayClickedSFX();
        settingsCanvas.SetActive(false);
    }
}
