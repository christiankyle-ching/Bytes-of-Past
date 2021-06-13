using System.Collections;
using System.Collections.Generic;
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

    // QUESTIONS DATA
    private Stack<QuestionData> questions = new Stack<QuestionData>();

    private QuestionData currentQuestion;

    private string[] currentChoices;

    private int currentScore = 0;

    private int totalQuestionCount;

    // INIT DATA
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
            Debug.Log("Static Data Not Found: Play from the Main Menu");
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
            TopicUtils.GetName((TOPIC) selectedTopic);

        QuestionData[] resourcesQuestions = null;

        switch (selectedTopic)
        {
            case TOPIC.Computer:
                resourcesQuestions =
                    Resources.LoadAll<QuestionData>("AssessmentTests/Computer");
                break;
            case TOPIC.Networking:
                resourcesQuestions =
                    Resources
                        .LoadAll<QuestionData>("AssessmentTests/Networking");
                break;
            case TOPIC.Software:
                resourcesQuestions =
                    Resources.LoadAll<QuestionData>("AssessmentTests/Software");
                break;
            default:
                resourcesQuestions =
                    Resources.LoadAll<QuestionData>("AssessmentTests");
                break;
        }

        foreach (QuestionData question in resourcesQuestions)
        {
            questions.Push (question);
        }

        totalQuestionCount = resourcesQuestions.Length;
    }

    void ShowNextQuestion()
    {
        Debug.Log($"SCORE: {currentScore}/{totalQuestionCount}");

        // TODO: If no questions left, show score and save and exit.
        if (questions.Count <= 0)
        {
            EndTest();
            return;
        }

        currentQuestion = questions.Pop();
        currentChoices = currentQuestion.Choices;

        string _questionText =
            $"Question: {totalQuestionCount - questions.Count}/" +
            totalQuestionCount +
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
    }

    void EndTest()
    {
        txtScore.GetComponent<TextMeshProUGUI>().text =
            $"Score: {currentScore}/{totalQuestionCount}";

        txtTopic.GetComponent<TextMeshProUGUI>().text =
            TopicUtils.GetName((TOPIC) selectedTopic);

        endGameMenu.SetActive(true);
    }

    void SaveAssessmentTestScore(TOPIC topic)
    {
        float score = currentScore / totalQuestionCount;

        // TODO: XML and Class Serializers might be better
        if (isPostAssessment)
        {
            PlayerPrefs.SetFloat($"Topic{topic}_Post_Score", score);
        }
        else
        {
            PlayerPrefs.SetFloat($"Topic{topic}_Score", score);
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
