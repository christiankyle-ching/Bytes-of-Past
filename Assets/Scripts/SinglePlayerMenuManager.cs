using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayerMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseButton;

    private void Awake()
    {
        InitEndMenu();
        InitPauseMenu();

        pauseButton
            .GetComponent<Button>()
            .onClick
            .AddListener(() => PauseGame());
    }

    // PauseGameMenu
    [SerializeField]
    private GameObject pauseMenuUI;

    private Button

            pause_ResumeButton,
            pause_RestartButton,
            pause_MenuButton,
            pause_QuitButton;

    private void InitPauseMenu()
    {
        pauseMenuUI.SetActive(false);

        pause_ResumeButton =
            pauseMenuUI.transform.Find("Resume").GetComponent<Button>();
        pause_RestartButton =
            pauseMenuUI.transform.Find("Restart").GetComponent<Button>();
        pause_MenuButton =
            pauseMenuUI.transform.Find("Menu").GetComponent<Button>();
        pause_QuitButton =
            pauseMenuUI.transform.Find("Quit").GetComponent<Button>();

        pause_ResumeButton.onClick.AddListener(() => ResumeGame());
        pause_RestartButton.onClick.AddListener(() => RestartGame());
        pause_MenuButton.onClick.AddListener(() => GotoMainMenu());
        pause_QuitButton.onClick.AddListener(() => QuitGame());
    }

    // EndGameMenu
    [SerializeField]
    private GameObject endMenuUI;

    private Button

            end_RestartButton,
            end_MenuButton,
            end_QuitButton;

    private void InitEndMenu()
    {
        endMenuUI.SetActive(false);

        end_RestartButton =
            endMenuUI.transform.Find("PlayAgain").GetComponent<Button>();
        end_MenuButton =
            endMenuUI.transform.Find("Menu").GetComponent<Button>();
        end_QuitButton =
            endMenuUI.transform.Find("Quit").GetComponent<Button>();

        end_RestartButton.onClick.AddListener(() => RestartGame());
        end_MenuButton.onClick.AddListener(() => GotoMainMenu());
        end_QuitButton.onClick.AddListener(() => QuitGame());
    }

    public void EndGame(PlayerStats stats)
    {
        endMenuUI.SetActive(true);
    }

    private void ResumeGame()
    {
        pauseButton.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    private void PauseGame()
    {
        pauseButton.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GotoMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
