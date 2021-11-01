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

    int preAssessmentTotalQuestions = 15;

    int postAssessmentTotalQuestions = 20;

    void Awake()
    {
        staticData = StaticData.Instance;
        ShowStatisticsData();
    }

    void ShowStatisticsData()
    {
        LoadAssessmentScores();
        LoadGameAccuracy();
        LoadSPGameWinLoss();
    }

    void LoadAssessmentScores()
    {
        string notDoneScore = "- / -";

        int[,] assessmentScores =
            staticData.profileStatisticsData.assessmentScores;

        // Assessment Scores
        for (int i = 0; i < 2; i++)
        {
            // [0] for PRE scores, [1] for POST scores
            bool loadingPreAssessment = i == 0;

            // Total Question Count based on gamemode index
            int totalQuestions = loadingPreAssessment ?
                preAssessmentTotalQuestions : postAssessmentTotalQuestions;

            // Assessment Done for each topic?
            bool assessDone1, assessDone2, assessDone3;

            if (loadingPreAssessment)
            {
                assessDone1 = PrefsConverter.IntToBoolean(
                    PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPreAssessmentDone(TOPIC.Computer), 0)
                );
                assessDone2 = PrefsConverter.IntToBoolean(
                    PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPreAssessmentDone(TOPIC.Networking), 0)
                );
                assessDone3 = PrefsConverter.IntToBoolean(
                    PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPreAssessmentDone(TOPIC.Software), 0)
                );
            }
            else
            {
                assessDone1 = PrefsConverter.IntToBoolean(
                    PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPostAssessmentDone(TOPIC.Computer), 0)
                );
                assessDone2 = PrefsConverter.IntToBoolean(
                    PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPostAssessmentDone(TOPIC.Networking), 0)
                );
                assessDone3 = PrefsConverter.IntToBoolean(
                    PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPostAssessmentDone(TOPIC.Software), 0)
                );
            }

            TextMeshProUGUI txtComputer = txtAssessment_Computer_Score[i].GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI txtNetworking = txtAssessment_Networking_Score[i].GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI txtSoftware = txtAssessment_Software_Score[i].GetComponent<TextMeshProUGUI>();

            txtComputer.text = assessDone1 ?
                $"{assessmentScores[i, (int)TOPIC.Computer]} / {totalQuestions}" :
                notDoneScore;

            txtNetworking.text = assessDone2 ?
                $"{assessmentScores[i, (int)TOPIC.Networking]} / {totalQuestions}" :
                notDoneScore;

            txtSoftware.text = assessDone3 ?
                $"{assessmentScores[i, (int)TOPIC.Software]} / {totalQuestions}" :
                notDoneScore;
        }
    }

    void LoadGameAccuracy()
    {
        float[,] spGameAccuracy = staticData.profileStatisticsData.SPGameAccuracy;
        float[] mpGameAccuracy = staticData.profileStatisticsData.MPGameAccuracy;

        int totalAccuracyCount = 0;
        float totalAccuracy = 0;
        foreach (float accuracy in spGameAccuracy)
        {
            if (accuracy != 0f)
            {
                totalAccuracy += accuracy;
                totalAccuracyCount++;
            }
        }
        foreach (float accuracy in mpGameAccuracy)
        {
            if (accuracy != 0f)
            {
                totalAccuracy += accuracy;
                totalAccuracyCount++;
            }
        }

        // If there aren't accuracies to be averaged, return 0f immediately to avoid division by 0 / 0
        float avgAccuracy =
            (totalAccuracyCount > 0) ? totalAccuracy / totalAccuracyCount : 0f;

        double avgAccuracyText = System.Math.Round((avgAccuracy * 100), 2);
        txtAccuracy.GetComponent<TextMeshProUGUI>().text = $"{avgAccuracyText}%";
    }

    void LoadSPGameWinLoss()
    {
        // FIXME: Possibly not working / accurate
        int[,,] SPWinLossCount =
            staticData.profileStatisticsData.SPWinLossCount;

        int[,] MPWinLossCount =
            staticData.profileStatisticsData.MPWinLossCount;

        int spWin = 0;
        int spLoss = 0;

        for (
            int topicIndex = 0;
            topicIndex < SPWinLossCount.GetLength(0);
            topicIndex++
        )
        {
            for (
                int difficultyIndex = 0;
                difficultyIndex < SPWinLossCount.GetLength(1);
                difficultyIndex++
            )
            {
                spWin += SPWinLossCount[topicIndex, difficultyIndex, 0];
                spLoss += SPWinLossCount[topicIndex, difficultyIndex, 1];
            }
        }

        int mpWin = 0;
        int mpLoss = 0;

        for (
            int topicIndex = 0;
            topicIndex < SPWinLossCount.GetLength(0);
            topicIndex++
        )
        {
            mpWin += MPWinLossCount[topicIndex, 0];
            mpLoss += MPWinLossCount[topicIndex, 1];
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
