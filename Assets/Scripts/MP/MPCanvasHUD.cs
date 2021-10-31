using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using System;

public class MPCanvasHUD : MonoBehaviour
{
    private GameObject endGameMenu, pauseMenu, bgMenu, btnPause;
    public SceneLoader sceneLoader;

    void Start()
    {
        // Local Menus
        endGameMenu = GameObject.Find("ENDGAME");
        pauseMenu = GameObject.Find("PAUSEMENU");
        bgMenu = GameObject.Find("BGMENU");
        btnPause = GameObject.Find("BTNPAUSE");
        endGameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        bgMenu.SetActive(false);
        btnPause.SetActive(true);
    }

    public void ShowEndGameMenu(Dictionary<uint, string> winners, bool interrupted = false, bool isHost = false, string interruptingPlayer = "")
    {
        bgMenu.SetActive(true);
        endGameMenu.SetActive(true);
        btnPause.SetActive(false);
        pauseMenu.SetActive(false);

        if (!interrupted)
        {
            bool gameWon = false;
            foreach (KeyValuePair<uint, string> winner in winners)
            {
                if (winner.Key == NetworkClient.localPlayer.netId)
                {
                    gameWon = true;
                    break;
                }
            }

            endGameMenu.transform.Find("WINSTATUS").GetComponent<TextMeshProUGUI>().text = gameWon ? "You Win" : "You Lose";

            List<string> winnerNames = new List<string>();
            foreach (KeyValuePair<uint, string> kvp in winners) { winnerNames.Add(kvp.Value); }

            endGameMenu.transform.Find("WinnerList").GetComponent<TextMeshProUGUI>().text = String.Join("\n", winnerNames);
        }
        else
        {
            endGameMenu.transform.Find("WINSTATUS").GetComponent<TextMeshProUGUI>().text = "ERROR";
            endGameMenu.transform.Find("WINNER").GetComponent<TextMeshProUGUI>().text = "";
            endGameMenu.transform.Find("WinnerList").GetComponent<TextMeshProUGUI>().text =
                (isHost) ? $"Player {interruptingPlayer} has quit." :
                "The server left the game";
            endGameMenu.transform.Find("BUTTONS").Find("BTNRESUME").gameObject.SetActive(false); // Disable Resume Button
        }
    }

    public void ShowInterruptedGame(string playerName, bool isHost)
    {
        ShowEndGameMenu(new Dictionary<uint, string> { }, true, isHost, playerName);
    }

    public void ShowPauseMenu()
    {
        endGameMenu.SetActive(false);
        pauseMenu.SetActive(true);
        bgMenu.SetActive(true);
        btnPause.SetActive(false);
    }

    public void CloseMenus()
    {
        endGameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        bgMenu.SetActive(false);
        btnPause.SetActive(true);
    }

    public void QuitGame()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        sceneLoader.GoToMainMenu();
    }
}
