using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssessmentManager : MonoBehaviour
{
    // GAMEOBJECT REFERENCES
    public GameObject questionNumberText;
    public GameObject questionText;

    public GameObject btnAnswer1;
    public GameObject btnAnswer2;
    public GameObject btnAnswer3;
    public GameObject btnAnswer4;

    public GameObject endGameMenu;
    public GameObject txtScore;
    public GameObject txtTopic;
    public GameObject txtTestTopic;
    public GameObject txtPreOrPostAssessment;

    public ResultIndicator resultIndicator;

    // QUESTIONS DATA
    private List<QuestionData> questions = new List<QuestionData>();

    private int currentQuestionIndex = 0;
    private QuestionData currentQuestion;
    private string[] currentChoices;

    private int currentScore = 0;

    // STATIC DATA
    private TOPIC selectedTopic;
    private bool isPostAssessment;
    StaticData staticData;

    void Awake()
    {
        staticData = StaticData.Instance;

        // Bind OnClick to functions
        btnAnswer1
         .GetComponent<Button>()
         .onClick
         .AddListener(() => SelectAnswer(0));

        btnAnswer2
         .GetComponent<Button>()
         .onClick
         .AddListener(() => SelectAnswer(1));

        btnAnswer3
         .GetComponent<Button>()
         .onClick
         .AddListener(() => SelectAnswer(2));

        btnAnswer4
         .GetComponent<Button>()
         .onClick
         .AddListener(() => SelectAnswer(3));

        LoadQuestions();
        ShowNextQuestion();
    }

    void LoadQuestions()
    {
        // Load Question depending on Topic
        selectedTopic = staticData.SelectedTopic;
        isPostAssessment = staticData.IsPostAssessment;

        Debug.Log($"Topic: {selectedTopic}");
        Debug.Log($"IsPostAssessment: {isPostAssessment}");

        txtTestTopic.GetComponent<TextMeshProUGUI>().text =
            "Topic: " + TopicUtils.GetName(selectedTopic);

        QuestionData[] resourcesQuestions = ResourceParser.Instance.ParseCSVToQuestions(selectedTopic);

        txtPreOrPostAssessment.GetComponent<TextMeshProUGUI>().text =
            isPostAssessment ? "Post-Assessment" : "Pre-Assessment";

        if (!isPostAssessment)
        {
            // If Pre-Assessment, load first 15 questions only
            questions = resourcesQuestions.Take(15).ToList();
        }
        else
        {
            // Else, shuffle the questions
            questions =
                resourcesQuestions.OrderBy(x => Random.Range(0f, 1f)).ToList();
        }
    }

    void ShowNextQuestion()
    {
        Debug.Log($"SCORE: {currentScore}/{questions.Count}");

        Debug.Log(currentQuestionIndex);
        Debug.Log(questions.Count);

        if (currentQuestionIndex >= questions.Count)
        {
            EndTest();
            return;
        }

        currentQuestion = questions.ElementAt(currentQuestionIndex);
        currentChoices = currentQuestion.Choices;

        questionNumberText.GetComponent<TextMeshProUGUI>().text = $"Question: {currentQuestionIndex + 1}/{questions.Count}";
        questionText.GetComponent<TextMeshProUGUI>().text = currentQuestion.Question;

        TextMeshProUGUI[] choicesTexts =
        {
            btnAnswer1.GetComponentInChildren<TextMeshProUGUI>(),
            btnAnswer2.GetComponentInChildren<TextMeshProUGUI>(),
            btnAnswer3.GetComponentInChildren<TextMeshProUGUI>(),
            btnAnswer4.GetComponentInChildren<TextMeshProUGUI>()
        };

        for (int i = 0; i < choicesTexts.Length; i++)
        {
            choicesTexts[i].text = currentChoices[i] ?? "NO DATA";
        }

        currentQuestionIndex++;
    }

    void EndTest()
    {
        txtScore.GetComponent<TextMeshProUGUI>().text =
            $"Score: {currentScore}/{questions.Count}";

        txtTopic.GetComponent<TextMeshProUGUI>().text =
            "Topic: " + TopicUtils.GetName((TOPIC)selectedTopic);

        endGameMenu.SetActive(true);

        // Save Score
        try
        {
            staticData
                .profileStatisticsData
                .UpdateAssessmentScore(isPostAssessment
                    ? GAMEMODE.PostAssessment
                    : GAMEMODE.PreAssessment,
                selectedTopic,
                currentScore);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save data: " + ex);
            Debug.LogError("Try starting debug on the main menu.");
        }

        if (!isPostAssessment)
        {
            PlayerPrefs
                .SetInt(TopicUtils
                    .GetPrefKey_IsPreAssessmentDone(selectedTopic),
                1);
        }
        else
        {
            PlayerPrefs
                .SetInt(TopicUtils
                    .GetPrefKey_IsPostAssessmentDone(selectedTopic),
                1);
        }
    }

    // Answer Methods

    void SelectAnswer(int index)
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[index]))
        {
            currentScore++;
            resultIndicator.ShowCorrect();
        }
        else
        {
            resultIndicator.ShowWrong();
        }

        ShowNextQuestion();
    }
}
