using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileStatisticsLoader : MonoBehaviour
{
    public GameObject txtSinglePlayer_Computer_Accuracy;

    public GameObject txtSinglePlayer_Networking_Accuracy;

    public GameObject txtSinglePlayer_Software_Accuracy;

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
}
