using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using LMAS.Scripts;
using LMAS.Scripts.Agent.Settings;

namespace LMAS.Scripts.Agent
{
    public enum BehaviorType
    {
        None,
        Move,
        Pickup,
        Drop,
        Talk,
        Interact,
    }

    [Serializable]
    public class BehaviorData
    {
        public string type;
        public string value;
    }

    public class LLMAgentBehavior : MonoBehaviour
    {
        public void Act(AgentInfo agent, float time, UnityEngine.Events.UnityAction<string> callback)
        {
            if (agent == null)
            {
                Debug.LogWarning("No agent found. Please load an agent first.");
                return;
            }

            StartCoroutine(ActCoroutine(agent.Name, time, callback));
        }

        private IEnumerator ActCoroutine(string agentName, float time, UnityEngine.Events.UnityAction<string> callback)
        {
            string result = string.Empty;
            string url = APISetting.APIUrl + $"/agent/{agentName}/act?current_time={time}";

            // AgentAPIClient apiClient = AgentAPIClient.Instance;
            // if (apiClient != null)
            // {
            //     var resp = apiClient.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json"));
            //     resp.EnsureSuccessStatusCode();
            //     var text = resp.Content.ReadAsStringAsync();
            //     result = text.Result;
            // }

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
                    Debug.LogError("Behavior request failed: " + request.error);
                }
            }
            callback?.Invoke(result);
        }
    }
}