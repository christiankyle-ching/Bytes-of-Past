using System.Collections;
using System.Linq;
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
            for (int i = 0; i < wrongAnswers.Length; i++) {
                choices[i] = wrongAnswers[i];
            }
            
            // Set Correct Answer
            choices[3] = correctAnswer;

            // Randomize Order
            IOrderedEnumerable<string> randomizedChoices = choices.OrderBy(x => Random.Range(0f, 1f));

            return randomizedChoices.ToArray();
        }
    }

    public bool isAnswerCorrect (string answer) {
        return answer.ToLower() == this.correctAnswer.ToLower();
    }

    public QuestionData(string question, string[] wrongAnswers, string correctAnswer) {
        this.question = question;
        this.wrongAnswers = wrongAnswers;
        this.correctAnswer = correctAnswer;
    }

    
}
