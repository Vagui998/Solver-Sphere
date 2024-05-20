using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Make sure you're using UnityEngine.UI
using TMPro;
using OpenAI;

public class OpenTextInteraction : MonoBehaviour
{
    public Canvas interactionCanvas; // Assign this in the Unity Editor.
    public Button closeButton; // Assign this in the Unity Editor.
    public TMP_InputField playerInputField; // Assign this in the Unity Editor.
   // public Text npcResponseText; // Assign this in the Unity Editor. Use TMP_Text if using TextMeshPro.
    public TMP_Text npcResponseText; // Uncomment if using TextMeshPro
    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();


    private void Start()
    {
        // Ensure the canvas and its components are not visible at the start.
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
        }

        // Assign the close button event.
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCanvas);
        }
    }

    public async void AskChatGPT()
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = playerInputField.text;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";
        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
            Debug.Log(chatResponse.Content);
            npcResponseText.text = chatResponse.Content;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is tagged as Player.
        if (other.CompareTag("Player"))
        {
            OpenCanvas();
            PauseGame();
        }
    }

    private void OpenCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(true);
            // Optionally clear previous text.
            if (playerInputField != null) playerInputField.text = "";
            if (npcResponseText != null) npcResponseText.text = "¡Hola!"; // Placeholder text
        }
    }

    private void CloseCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Pauses the game.
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Resumes the game.
    }
}
