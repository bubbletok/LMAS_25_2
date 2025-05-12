using UnityEngine;
using LMAS.Scripts.Agent.Settings;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace LMAS.Scripts.Agent
{
    public class LLMAgentVAD : MonoBehaviour
    {
        public void AnalyzeEmotion(AgentInfo agent, string prompt, UnityAction<string> callback)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                Debug.LogWarning("Please enter a prompt.");
                return;
            }

            if (agent == null)
            {
                Debug.LogWarning("No agent found. Please load an agent first.");
                return;
            }

            StartCoroutine(AnalyzeEmotionCoroutine(agent.Name, prompt, callback));
        }

        private IEnumerator AnalyzeEmotionCoroutine(string agentName, string promptText, UnityAction<string> callback)
        {
            string result = string.Empty;
            string url = APISetting.APIUrl + $"/agent/{agentName}/analyze_emotion?prompt={UnityWebRequest.EscapeURL(promptText)}";
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    result = responseJson;
                }
                else
                {
                    result = "API request failed: " + request.error;
                    Debug.LogError("Post request failed: " + request.error);
                }
            }
            callback?.Invoke(result);
        }

        public void GenerateEmotion(AgentInfo agent, string prompt, UnityAction<string> callback)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                Debug.LogWarning("Please enter a prompt.");
                return;
            }

            if (agent == null)
            {
                Debug.LogWarning("No agent found. Please load an agent first.");
                return;
            }

            StartCoroutine(GenerateEmotionCoroutine(agent.Name, prompt, callback));
        }

        private IEnumerator GenerateEmotionCoroutine(string agentName, string promptText, UnityAction<string> callback)
        {
            string result = string.Empty;
            string url = APISetting.APIUrl + $"/agent/{agentName}/generate_emotion?prompt={UnityWebRequest.EscapeURL(promptText)}";
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    result = responseJson;
                }
                else
                {
                    result = "API request failed: " + request.error;
                    Debug.LogError("Post request failed: " + request.error);
                }
            }
            callback?.Invoke(result);
        }
    }
}