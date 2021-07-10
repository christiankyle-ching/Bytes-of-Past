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
        try
        {
            staticData =
                GameObject
                    .FindWithTag("Static Data")
                    .GetComponent<StaticData>();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Static Data Not Found: Play from the Main Menu");
            staticData = new StaticData();
        }

        // Bind OnClick to functions
        btnAnswer1
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer1());

        btnAnswer2
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer2());

        btnAnswer3
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer3());

        btnAnswer4
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer4());

        LoadQuestions();
        ShowNextQuestion();
    }

    void LoadQuestions()
    {
        // Load Question depending on Topic
        selectedTopic = staticData.SelectedTopic;
        isPostAssessment = staticData.IsPostAssessment;

        txtTestTopic.GetComponent<TextMeshProUGUI>().text =
            "Topic: " + TopicUtils.GetName(selectedTopic);

        QuestionData[] resourcesQuestions = ParseCSVToQuestions(TOPIC.Computer);

        txtPreOrPostAssessment.GetComponent<TextMeshProUGUI>().text =
            isPostAssessment ? "Post-Assessment" : "Pre-Assessment";

        if (!isPostAssessment)
        {
            // If Pre-Assessment, load first 5 questions only
            questions = resourcesQuestions.Take(5).ToList();
        }
        else
        {
            // Else, shuffle the questions
            questions =
                resourcesQuestions.OrderBy(x => Random.Range(0f, 1f)).ToList();
        }
    }

    QuestionData[] ParseCSVToQuestions(TOPIC topic)
    {
        /* 
        IMPORTANT: Download from Google Sheets in .tsv. Then RENAME format to .csv.
        Then drag it to Unity (to be recognized as a TextAsset with tabs as delimiters).
        */
        List<QuestionData> questions = new List<QuestionData>();

        // Parse a CSV file containing the questions and answers
        TextAsset rawData = null;

        switch (topic)
        {
            case TOPIC.Computer:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("AssessmentTests/Assessment Questions - Computers");
                break;
            case TOPIC.Networking:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("AssessmentTests/Assessment Questions - Networking");
                break;
            case TOPIC.Software:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("AssessmentTests/Assessment Questions - Software");
                break;
        }

        if (rawData != null)
        {
            string[] lines = rawData.text.Split('\n'); // split into lines

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0) continue; // ignore header

                string[] cells = lines[i].Split('\t');

                // Cell 0 : ID
                // Cell 1 : Question
                // Cell 2 to 4 : Wrong Answers (3)
                // Cell 5 : Correct Answers
                questions
                    .Add(new QuestionData(cells[1],
                        cells.Skip(2).Take(3).ToArray(),
                        cells[5]));
            }
        }

        return questions.ToArray();
    }

    void ShowNextQuestion()
    {
        Debug.Log($"SCORE: {currentScore}/{questions.Count}");

        Debug.Log (currentQuestionIndex);
        Debug.Log(questions.Count);

        if (currentQuestionIndex >= questions.Count)
        {
            EndTest();
            return;
        }

        currentQuestion = questions.ElementAt(currentQuestionIndex);
        currentChoices = currentQuestion.Choices;

        string _questionText =
            $"Question: {currentQuestionIndex + 1}/" +
            questions.Count +
            "\n\n" +
            currentQuestion.Question;

        questionText.GetComponent<TextMeshProUGUI>().text = _questionText;

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
            "Topic: " + TopicUtils.GetName((TOPIC) selectedTopic);

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
    void SelectAnswer1()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[0]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer2()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[1]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer3()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[2]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer4()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[3]))
        {
            currentScore++;
        }
        ShowNextQuestion();
    }
}
