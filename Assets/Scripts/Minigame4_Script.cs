using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using UnityEngine.UI;
using Unity.Services.CloudSave;  // Include Cloud Save namespace

public class Minigame4_Script : MonoBehaviour
{
    public GameObject quizPanel;
    public TMP_Text questionText;
    public TMP_Text[] answerTexts;
    public Button[] answerButtons;
    public TMP_Text scoreTxt;

    private List<Question> questions;
    private int currentQuestionIndex = 0;
    private int score = 0; // Score keeper

    void Start()
    {
        // Load questions from CSV file
        LoadQuestionsFromCSV("Minigame4.csv");

        // Hide quiz panel initially
        quizPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Freeze the player
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            // Show quiz panel
            quizPanel.SetActive(true);
            scoreTxt.text = "Puntaje : " + score;
            DisplayCurrentQuestion();
        }
    }

    void LoadQuestionsFromCSV(string filePath)
    {
        questions = new List<Question>();

        try
        {
            StreamReader reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                Question question = new Question();
                question.ID = int.Parse(values[0]);
                question.questionText = values[1];
                question.optionA = values[2];
                question.optionB = values[3];
                question.optionC = values[4];
                question.optionD = values[5];
                question.correctAnswer = values[6][0];
                question.category = values[7];

                questions.Add(question);
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading questions: " + e.Message);
        }
    }

    void DisplayCurrentQuestion()
    {
        Question currentQuestion = questions[currentQuestionIndex];
        questionText.text = currentQuestion.questionText;
        answerTexts[0].text = currentQuestion.optionA;
        answerTexts[1].text = currentQuestion.optionB;
        answerTexts[2].text = currentQuestion.optionC;
        answerTexts[3].text = currentQuestion.optionD;
    }

    public void CheckAnswer(int buttonIndex)
    {
        char correctAnswer = questions[currentQuestionIndex].correctAnswer;

        if (buttonIndex == correctAnswer - 'A')
        {
            answerButtons[buttonIndex].image.color = Color.green;
            score++; // Increment score for correct answer
            scoreTxt.text = "Puntaje : " + score;
        }
        else
        {
            answerButtons[correctAnswer - 'A'].image.color = Color.green;
            answerButtons[buttonIndex].image.color = Color.red;
        }

        StartCoroutine(ShowResult());
    }

    IEnumerator ShowResult()
    {
        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }

        yield return new WaitForSeconds(2f);

        foreach (Button button in answerButtons)
        {
            button.image.color = Color.white;
            button.interactable = true;
        }

        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            DisplayCurrentQuestion();
        }
        else
        {
            // When the quiz is finished, save the score multiplied by 125
            SaveScore(score * 125);
            quizPanel.SetActive(false);
            Debug.Log("Quiz finished! Score: " + score);
        }
    }

    // Save the score using Unity Cloud Save
    // Save the score using Unity Cloud Save
    private async void SaveScore(int finalScore)
    {
        try
        {
            // Prepare the data to be saved
            var data = new Dictionary<string, object> { { "MinigameScore", finalScore } };

            // Save the data
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Score saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save score: " + e.Message);
        }
    }

}

[System.Serializable]
public class Question
{
    public int ID;
    public string questionText;
    public string optionA;
    public string optionB;
    public string optionC;
    public string optionD;
    public char correctAnswer;
    public string category;
}
