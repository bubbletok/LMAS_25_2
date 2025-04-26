using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class PromptRequest
{
    public string prompt;
}

[Serializable]
public class PromptResponse
{
    public string status;
    public string message;
    public string response;
}

public class LLMApiClient : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiUrl = "http://localhost:8000/prompt";
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField promptInputField;
    [SerializeField] private TMP_Text responseText;
    [SerializeField] private Button submitButton;

    private void Start()
    {
        // Connect button event
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SendPrompt);
        }
    }

    public void SendPrompt()
    {
        if (string.IsNullOrWhiteSpace(promptInputField.text))
        {
            Debug.LogWarning("Please enter a prompt.");
            return;
        }

        StartCoroutine(SendPromptCoroutine(promptInputField.text));
    }

    private IEnumerator SendPromptCoroutine(string promptText)
    {
        // Update UI state
        if (submitButton != null) submitButton.interactable = false;
        if (responseText != null) responseText.text = "Waiting for response...";

        // Prepare request data
        PromptRequest requestData = new PromptRequest
        {
            prompt = promptText
        };
        
        string jsonData = JsonUtility.ToJson(requestData);
        
        // Create API request
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send request and wait for response
            yield return request.SendWebRequest();

            // Restore UI state
            if (submitButton != null) submitButton.interactable = true;

            // Process response
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseJson = request.downloadHandler.text;
                    PromptResponse response = JsonUtility.FromJson<PromptResponse>(responseJson);
                    
                    if (responseText != null)
                    {
                        responseText.text = response.response;
                    }
                    
                    Debug.Log("API Response: " + response.response);
                }
                catch (Exception e)
                {
                    if (responseText != null)
                    {
                        responseText.text = "Error occurred while processing response.";
                    }
                    Debug.LogError("Response processing error: " + e.Message);
                }
            }
            else
            {
                if (responseText != null)
                {
                    responseText.text = "API request failed: " + request.error;
                }
                Debug.LogError("API request failed: " + request.error);
            }
        }
    }
}