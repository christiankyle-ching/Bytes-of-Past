using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileStatisticsLoader : MonoBehaviour
{
    public GameObject txtAccuracy;

    public GameObject txtTotalGames;

    public GameObject txtSPWinOrLoss;

    public GameObject txtMPWinOrLoss;

    // ASSESSMENT SCORES
    // Size of [2]
    // 2 for Pre & Post Assessment
    public GameObject[] txtAssessment_Computer_Score;

    public GameObject[] txtAssessment_Networking_Score;

    public GameObject[] txtAssessment_Software_Score;

    StaticData staticData;

    int preAssessmentTotalQuestions = 5;

    int postAssessmentTotalQuestions = 20;

    void Awake()
    {
        try
        {
            staticData =
                GameObject
                    .FindWithTag("Static Data")
                    .GetComponent<StaticData>();

            ShowStatisticsData();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Static Data Not Found: Play from the Main Menu");
            staticData = new StaticData();
        }
    }

    void ShowStatisticsData()
    {
        LoadAssessmentScores();
        LoadGameAccuracy();
        LoadGameWinLoss();
    }

    void LoadAssessmentScores()
    {
        int[,] assessmentScores =
            staticData.profileStatisticsData.assessmentScores;

        // Assessment Scores
        for (int i = 0; i < 2; i++)
        {
            // Total Question Count based on gamemode index
            int totalQuestions =
                (
                (i == 0)
                    ? preAssessmentTotalQuestions
                    : postAssessmentTotalQuestions
                );

            string label = (i == 0) ? "Pre-Assessment: " : "Post-Assessment: ";

            txtAssessment_Computer_Score[i]
                .GetComponent<TextMeshProUGUI>()
                .text =
                label +
                assessmentScores[i, (int) TOPIC.Computer].ToString() +
                "/" +
                totalQuestions;

            txtAssessment_Networking_Score[i]
                .GetComponent<TextMeshProUGUI>()
                .text =
                label +
                assessmentScores[i, (int) TOPIC.Networking].ToString() +
                "/" +
                totalQuestions;

            txtAssessment_Software_Score[i]
                .GetComponent<TextMeshProUGUI>()
                .text =
                label +
                assessmentScores[i, (int) TOPIC.Software].ToString() +
                "/" +
                totalQuestions;
        }
    }

    void LoadGameAccuracy()
    {
        float[,,] gameAccuracy = staticData.profileStatisticsData.gameAccuracy;

        int totalAccuracyCount = 0;
        float totalAccuracy = 0;
        foreach (float accuracy in gameAccuracy)
        {
            if (accuracy != 0f)
            {
                totalAccuracy += accuracy;
                totalAccuracyCount++;
            }
        }

        // If there aren't accuracies to be averaged, return 0f immediately to avoid division by 0
        float avgAccuracy =
            (totalAccuracyCount > 0) ? totalAccuracy / totalAccuracyCount : 0f;

        txtAccuracy.GetComponent<TextMeshProUGUI>().text =
            (avgAccuracy * 100) + "%";
    }

    void LoadGameWinLoss()
    {
        int[,,,] gameWinLossCount =
            staticData.profileStatisticsData.gameWinLossCount;

        // gameWinLossCount[0] are all GAMEMODE.SinglePlayer games
        int spWin = 0;
        int spLoss = 0;

        for (
            int topicIndex = 0;
            topicIndex < gameWinLossCount.GetLength(1);
            topicIndex++
        )
        {
            for (
                int difficultyIndex = 0;
                difficultyIndex < gameWinLossCount.GetLength(2);
                difficultyIndex++
            )
            {
                spWin += gameWinLossCount[0, topicIndex, difficultyIndex, 0];
                spLoss += gameWinLossCount[0, topicIndex, difficultyIndex, 0];
            }
        }

        // gameWinLossCount[1] are all GAMEMODE.Multiplayer games
        int mpWin = 0;
        int mpLoss = 0;

        for (
            int topicIndex = 0;
            topicIndex < gameWinLossCount.GetLength(1);
            topicIndex++
        )
        {
            for (
                int difficultyIndex = 0;
                difficultyIndex < gameWinLossCount.GetLength(2);
                difficultyIndex++
            )
            {
                mpWin += gameWinLossCount[1, topicIndex, difficultyIndex, 0];
                mpLoss += gameWinLossCount[1, topicIndex, difficultyIndex, 0];
            }
        }

        txtSPWinOrLoss.GetComponent<TextMeshProUGUI>().text =
            $"{spWin}W / {spLoss}L";
        txtMPWinOrLoss.GetComponent<TextMeshProUGUI>().text =
            $"{mpWin}W / {mpLoss}L";

        int totalGames = spWin + spLoss + mpWin + mpLoss;

        txtTotalGames.GetComponent<TextMeshProUGUI>().text =
            totalGames + " game/s";
    }
}
