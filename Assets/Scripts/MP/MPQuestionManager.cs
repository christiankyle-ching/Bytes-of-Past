using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(CanvasGroup))]
public class MPQuestionManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private MPGameMessage messenger;

    [Header("GameObject References")]
    public Transform buttonsContainer;
    public TextMeshProUGUI questionText;

    private string question;
    private string[] choices;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        messenger = GameObject.Find("MESSENGER").GetComponent<MPGameMessage>();

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
        NetworkIdentity ni = NetworkClient.localPlayer;
        PlayerManager pm = ni.GetComponent<PlayerManager>();

        pm.AnswerQuiz(this.choices[index]);

        SetVisibility(false);
    }

    public void ShowQuestion(string question, string[] chs)
    {
        // Set UI
        questionText.text = question;
        this.choices = chs;

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
