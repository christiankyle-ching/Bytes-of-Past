using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssessmentManager : MonoBehaviour
{
    public GameObject QuestionText;

    public GameObject BtnAnswer1;

    public GameObject BtnAnswer2;

    public GameObject BtnAnswer3;

    private Stack<QuestionData> questions = new Stack<QuestionData>();

    private QuestionData currentQuestion;

    private string[] currentChoices;

    private int correctAnswerCount = 0;

    private int totalQuestionCount;

    void Awake()
    {
        BtnAnswer1
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer1());

        BtnAnswer2
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer2());

        BtnAnswer3
            .GetComponent<Button>()
            .onClick
            .AddListener(() => SelectAnswer3());

        LoadQuestions();
        ShowNextQuestion();
    }

    void LoadQuestions()
    {
        // TODO: Apply Different Topics
        QuestionData[] resourcesQuestions =
            Resources.LoadAll<QuestionData>("AssessmentTests/Computer");

        foreach (QuestionData question in resourcesQuestions)
        {
            questions.Push (question);
        }

        totalQuestionCount = resourcesQuestions.Length;
    }

    void ShowNextQuestion()
    {
        Debug.Log("SCORE: " + correctAnswerCount + "/" + totalQuestionCount);

        // TODO: If no questions left, show score and save and exit.
        if (questions.Count <= 0) return;

        currentQuestion = questions.Pop();
        currentChoices = currentQuestion.Choices;

        QuestionText.GetComponent<TextMeshProUGUI>().text =
            currentQuestion.Question;
        BtnAnswer1.GetComponentInChildren<TextMeshProUGUI>().text =
            currentChoices[0] ?? "NO DATA";
        BtnAnswer2.GetComponentInChildren<TextMeshProUGUI>().text =
            currentChoices[1] ?? "NO DATA";
        BtnAnswer3.GetComponentInChildren<TextMeshProUGUI>().text =
            currentChoices[2] ?? "NO DATA";
    }

    void SelectAnswer1()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[0]))
        {
            correctAnswerCount++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer2()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[1]))
        {
            correctAnswerCount++;
        }
        ShowNextQuestion();
    }

    void SelectAnswer3()
    {
        if (currentQuestion.isAnswerCorrect(currentChoices[2]))
        {
            correctAnswerCount++;
        }
        ShowNextQuestion();
    }
}
