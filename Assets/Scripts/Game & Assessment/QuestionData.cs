using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class QuestionData : ScriptableObject
{
    [SerializeField]
    private string question;

    [SerializeField]
    private string[] wrongAnswers;

    [SerializeField]
    private string correctAnswer;

    public string Question {
        get => this.question;
    }

    public string CorrectAnswer {
        get => this.correctAnswer;
    }

    public string[] Choices {
        get {
            string[] choices = new string[4];

            // Set Wrong Answers
            for (int i = 0; i < choices.Length; i++) {
                choices[i] = wrongAnswers[Random.Range(0, wrongAnswers.Length-1)];
            }

            // Set Correct Answer
            int correctAnswerPos = Random.Range(0,4);
            choices[correctAnswerPos] = correctAnswer;

            return choices;
        }
    }

    public bool isAnswerCorrect (string answer) {
        return answer.ToLower() == this.correctAnswer.ToLower();
    }

    
}
