using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LMAS.Scripts.Agent.Settings;
using UnityEngine;
using UnityEngine.Networking;
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

        /// <summary>
        /// 한 줄로 주어진 Behavior 정의에서, 첫 번째 토큰(공백 전단어)만 뽑아서 리턴합니다.
        /// 예:
        ///   "Move x: 10 y: 20"  → "Move"
        ///   "Pickup"            → "Pickup"
        ///   "Talk to Bob. ..."  → "Talk"
        ///   "None"              → "None"
        /// </summary>
        /// <param name="line">파싱할 Behavior 정의 한 줄</param>
        /// <returns>첫 번째 단어(Behavior의 type) 혹은 빈 문자열</returns>
        public BehaviorType ParseBehaviorType(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return BehaviorType.None;

            // 앞뒤 공백 제거
            string trimmed = line.Trim();

            // 앞뒤 "" 제거
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
            {
                trimmed = trimmed.Substring(1, trimmed.Length - 2).Trim();
            }

            // 공백(스페이스) 기준으로 분리해서 첫 번째 토큰만 가져온다.
            int firstSpaceIdx = trimmed.IndexOf(' ');
            string finalType = string.Empty;
            if (firstSpaceIdx < 0)
            {
                // 공백이 없으면, 전체가 type
                finalType = trimmed;
            }
            else
            {
                // 공백 이전 부분만 type
                finalType = trimmed.Substring(0, firstSpaceIdx);
            }

            if (Enum.TryParse(finalType, true, out BehaviorType behaviorType))
            {
                return behaviorType; // 성공적으로 파싱된 경우
            }
            else
            {
                Debug.LogWarning($"Unknown behavior type: {finalType}");
                return BehaviorType.None; // 알 수 없는 타입인 경우

            }
        }
    }
}