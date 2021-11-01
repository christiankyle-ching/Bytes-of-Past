using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SPQuestionManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("GameObject References")]
    public SinglePlayerGameController gameController;
    public Transform buttonsContainer;
    public TextMeshProUGUI questionText;

    private QuestionData question;
    private string[] choices;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Set Listeners
        for (int i = 0; i < buttonsContainer.childCount; i++)
        {
            int index = i; // Needed line! See this: http://answers.unity.com/answers/1580342/view.html

            buttonsContainer.GetChild(i).GetComponent<Button>().onClick.AddListener(
                () => SelectAnswer(index));
        }

        SetVisibility(false);
    }

    private void SelectAnswer(int index)
    {
        gameController.AnswerQuiz(choices[index]);
        SetVisibility(false);
    }

    public void ShowQuestion(QuestionData _q)
    {
        question = _q;

        // Set UI
        questionText.text = _q.Question;
        choices = _q.Choices;

        for (int i = 0; i < choices.Length; i++)
        {
            buttonsContainer.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
        }

        SetVisibility(true);
    }

    public void SetVisibility(bool enabled)
    {
        if (enabled)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1.0f;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }
    }
}
