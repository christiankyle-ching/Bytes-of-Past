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

    public void ShowEndGameMenu(string[] winnerNames, NetworkIdentity[] winnerIdens, bool interrupted = false, string interruptingPlayer = "")
    {
        bgMenu.SetActive(true);
        endGameMenu.SetActive(true);
        btnPause.SetActive(false);
        pauseMenu.SetActive(false);

        if (!interrupted)
        {
            bool gameWon = false;
            foreach (NetworkIdentity iden in winnerIdens)
            {
                if (iden.isLocalPlayer)
                {
                    gameWon = true;
                    break;
                }
            }

            endGameMenu.transform.Find("WINSTATUS").GetComponent<TextMeshProUGUI>().text = gameWon ? "You Win" : "You Lose";
            endGameMenu.transform.Find("WinnerList").GetComponent<TextMeshProUGUI>().text = String.Join("\n", winnerNames);
        }
        else
        {
            endGameMenu.transform.Find("WINSTATUS").GetComponent<TextMeshProUGUI>().text = "ERROR";
            endGameMenu.transform.Find("WinnerList").GetComponent<TextMeshProUGUI>().text = $"Player {interruptingPlayer} has quit.";
            endGameMenu.transform.Find("BUTTONS").GetChild(0).gameObject.SetActive(false); // Disable Resume Button
        }
    }

    public void ShowInterruptedGame(string playerName)
    {
        ShowEndGameMenu(new string[0], new NetworkIdentity[0], true, playerName);
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
