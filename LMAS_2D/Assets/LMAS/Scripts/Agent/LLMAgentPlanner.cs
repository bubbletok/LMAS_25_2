using System.Collections;
using LMAS.Scripts.Agent.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace LMAS.Scripts.Agent
{
    public class LLMAgentPlanner : MonoBehaviour
    {
        string m_RecentPlan;
        [HideInInspector] public string RecentPlan => m_RecentPlan;

        public void Plan(AgentInfo agent, float time, UnityAction<string> callback = null)
        {
            if (agent == null)
            {
                Debug.LogWarning("No agent found. Please load an agent first.");
                return;
            }

            StartCoroutine(PlanCoroutine(agent.Name, time, callback));
        }

        private IEnumerator PlanCoroutine(string agentName, float time, UnityAction<string> callback = null)
        {
            string result = string.Empty;
            string url = APISetting.APIUrl + $"/agent/{agentName}/plan?current_time={time}";
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    result = responseJson;
                    m_RecentPlan = result;
                }
                else
                {
                    Debug.LogError("Plan request failed: " + request.error);
                }
            }
            callback?.Invoke(result);
        }
    }
}
