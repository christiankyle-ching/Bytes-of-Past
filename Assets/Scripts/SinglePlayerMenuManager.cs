using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayerMenuManager : MonoBehaviour
{
    private void Awake()
    {
        InitEndMenu();
    }

    // EndGameMenu
    [SerializeField]
    private GameObject endMenuUI;
    private Button end_RestartButton, end_MenuButton, end_QuitButton;
    private void InitEndMenu()
    {
        endMenuUI.SetActive(false);

        end_RestartButton = endMenuUI.transform.Find("PlayAgain").GetComponent<Button>();
        end_MenuButton = endMenuUI.transform.Find("Menu").GetComponent<Button>();
        end_QuitButton = endMenuUI.transform.Find("Quit").GetComponent<Button>();

        end_RestartButton.onClick.AddListener(() => RestartGame());
        end_MenuButton.onClick.AddListener(() => GotoMainMenu());
        end_QuitButton.onClick.AddListener(() => QuitGame());
    }

    public void EndGame(PlayerStats stats)
    {
        endMenuUI.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("SinglePlayerScene");
    }

    private void GotoMainMenu()
    {
        throw new NotImplementedException();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
