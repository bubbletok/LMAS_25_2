using System.Collections;
using LMAS.Scripts.Agent.Settings;
using LMAS.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace LMAS.Scripts.Agent
{
    public class LLMAgentMemory : MonoBehaviour
    {
        public void Observe(AgentInfo agent, string prompt, float time, UnityAction<string> callback = null)
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

            StartCoroutine(ObserveCoroutine(agent.Name, prompt, time, callback));
        }

        IEnumerator ObserveCoroutine(string agentName, string promptText, float time, UnityAction<string> callback)
        {
            string result = string.Empty;
            string url = APISetting.APIUrl + $"/agent/{agentName}/observe?observation={UnityWebRequest.EscapeURL(promptText)}&time={time}";
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

        public void GetSummary(AgentInfo agent, UnityAction<string> callback = null)
        {
            if (agent == null)
            {
                Debug.LogWarning("No agent found. Please load an agent first.");
                return;
            }

            StartCoroutine(GetSummaryCoroutine(agent.Name, callback));
        }

        IEnumerator GetSummaryCoroutine(string agentName, UnityAction<string> callback)
        {
            string result = string.Empty;
            string url = APISetting.APIUrl + $"/agent/{agentName}/summary";
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