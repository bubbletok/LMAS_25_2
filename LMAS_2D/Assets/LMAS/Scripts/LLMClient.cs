using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Unity.Burst.Intrinsics;

namespace LMAS
{
    [Serializable]
    public class LLM
    {
        [JsonProperty("verbose")]
        public bool Verbose;
        [JsonProperty("base_url")]
        public string BaseUrl;
        [JsonProperty("model")]
        public string Model;
        [JsonProperty("disable_streaming")]
        public bool DisableStreaming;

        public override string ToString()
        {
            return $"Verbose: {Verbose}, BaseUrl: {BaseUrl}, Model: {Model}, DisableStreaming: {DisableStreaming}";
        }
    }
    [Serializable]
    public class AgentChat
    {
        [JsonProperty("llm")]
        public LLM LLM;

        public override string ToString()
        {
            return $"LLM: {LLM}";
        }
    }
    [Serializable]
    public class AgentMemory
    {
        [JsonProperty("working_memory")]
        public List<string> WorkingMemory;
        [JsonProperty("mid_term_memory")]
        public List<string> MiddleTermMemory;
        [JsonProperty("long_term_memory")]
        public List<string> LongTermMemory;
    }
    [Serializable]
    public class AgentBehavior
    {
        [JsonProperty("recent_summary")]
        public string RecentSummary;
    }

    [Serializable]
    public class AgentPlanner
    {
        [JsonProperty("plan")]
        public Dictionary<string, string> Plan;
    }

    [Serializable]
    public class AgentVAD
    {
        [JsonProperty("valence")]
        public Dictionary<string, float> Valence = new Dictionary<string, float>();
        [JsonProperty("arousal")]
        public Dictionary<string, float> Arousal = new Dictionary<string, float>();
        [JsonProperty("dominance")]
        public Dictionary<string, float> Dominance = new Dictionary<string, float>();
        [JsonProperty("emotion")]
        public Dictionary<string, List<string>> Emotion = new Dictionary<string, List<string>>();
        [JsonProperty("mood")]
        public Dictionary<string, string> Mood = new Dictionary<string, string>();
        [JsonProperty("chat")]
        public AgentChat Chat;
    }

    [Serializable]
    public class AgentInfo
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("age")]
        public int Age;
        [JsonProperty("chat")]
        public AgentChat Chat;
        [JsonProperty("memory")]
        public AgentMemory Memory;
        [JsonProperty("behavior")]
        public AgentBehavior Behavior;
        [JsonProperty("planner")]
        public AgentPlanner Planner;
        [JsonProperty("vad")]
        public AgentVAD Vad;
    }

    [Serializable]
    public class Response
    {
        [JsonProperty("content")]
        public string Content;
    }

    [Serializable]
    public class TestChatResponse
    {
        [JsonProperty("agent")]
        public string Name;
        [JsonProperty("response")]
        public Response Response;
    }

    public class LLMClient : MonoBehaviour
    {
        // [Header("Agent Settings")]
        // [SerializeField] private string agentName = "maru";

        [Header("UI References")]
        [SerializeField] private TMP_InputField m_currentCircumInputField;
        [SerializeField] private TMP_Text m_analyzeEmotionResponseText;
        [SerializeField] private Button m_analyzeEmotionButton;
        [SerializeField] private TMP_Text m_generateEmotionResponseText;
        [SerializeField] private Button m_generateEmotionButton;

        [SerializeField] private TMP_InputField m_getAgentInfoInputField;
        [SerializeField] private Button m_getAgentInfoButton;

        private AgentInfo m_currentAgent;
        private Vector2 m_scrollPos = Vector2.zero;

        // GUIStyles
        GUIStyle sectionHeaderStyle;
        GUIStyle labelStyle;
        GUIStyle boxStyle;
        #region Unity Methods
        private void Start()
        {
            // Connect button event
            if (m_analyzeEmotionButton != null)
            {
                m_analyzeEmotionButton.onClick.AddListener(AnalyzeEmotion);
            }
            if (m_generateEmotionButton != null)
            {
                m_generateEmotionButton.onClick.AddListener(GenerateEmotion);
            }

            if (m_getAgentInfoButton != null)
            {
                m_getAgentInfoButton.onClick.AddListener(GetAgentInfo);
            }
        }
        #endregion

        public void AnalyzeEmotion()
        {
            string prompt = m_currentCircumInputField.text;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                Debug.LogWarning("Please enter a prompt.");
                return;
            }

            if (m_currentAgent == null)
            {
                Debug.LogWarning("No agent loaded. Please load an agent first.");
                return;
            }

            StartCoroutine(AnalyzeEmotionCoroutine(m_currentAgent.Name, prompt));
        }

        private IEnumerator AnalyzeEmotionCoroutine(string agentName, string promptText)
        {
            m_analyzeEmotionButton.interactable = false;
            m_analyzeEmotionResponseText.text = "Waiting for response...";

            string url = APISetting.APIUrl + $"/agent/{agentName}/analyze_emotion?prompt={UnityWebRequest.EscapeURL(promptText)}";
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                m_analyzeEmotionButton.interactable = true;
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    m_analyzeEmotionResponseText.text = responseJson;
                }
                else
                {
                    m_analyzeEmotionResponseText.text = "API request failed: " + request.error;
                    Debug.LogError("Post request failed: " + request.error);
                }
            }
        }

        public void GenerateEmotion()
        {
            string prompt = m_currentCircumInputField.text;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                Debug.LogWarning("Please enter a prompt.");
                return;
            }

            if (m_currentAgent == null)
            {
                Debug.LogWarning("No agent loaded. Please load an agent first.");
                return;
            }

            StartCoroutine(GenerateEmotionCoroutine(m_currentAgent.Name, prompt));
        }

        private IEnumerator GenerateEmotionCoroutine(string agentName, string promptText)
        {
            m_generateEmotionButton.interactable = false;
            m_generateEmotionResponseText.text = "Waiting for response...";

            string url = APISetting.APIUrl + $"/agent/{agentName}/generate_emotion?prompt={UnityWebRequest.EscapeURL(promptText)}";
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                m_generateEmotionButton.interactable = true;
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    m_generateEmotionResponseText.text = responseJson;
                }
                else
                {
                    m_generateEmotionResponseText.text = "API request failed: " + request.error;
                    Debug.LogError("Post request failed: " + request.error);
                }
            }
        }

        public void GetAgentInfo()
        {
            string agentName = m_getAgentInfoInputField.text;
            if (string.IsNullOrWhiteSpace(agentName))
            {
                Debug.LogWarning("Please enter a valid agent name.");
                return;
            }
            StartCoroutine(GetAgentInfoCoroutine(agentName));
        }

        private IEnumerator GetAgentInfoCoroutine(string agentName)
        {
            string requestUrl = APISetting.APIUrl + $"/agent/{agentName}";
            Debug.Log("Requesting agent info from: " + requestUrl);
            using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AgentInfo agentInfo = JsonConvert.DeserializeObject<AgentInfo>(request.downloadHandler.text);
                    m_currentAgent = agentInfo; // Store the current agent info
                    // string responseJson = request.downloadHandler.text;
                    // Debug.Log("Agent Info: " + responseJson);
                    Debug.Log($"Name: {agentInfo.Name} Age: {agentInfo.Age}");

                    Debug.Log($"Chat: {agentInfo.Chat}");
                    Debug.Log($"LLM: {agentInfo.Chat.LLM}");

                    Debug.Log($"Memory: {string.Join(", ", agentInfo.Memory)}");
                    Debug.Log($"Working Memory: {string.Join(", ", agentInfo.Memory.WorkingMemory)}");
                    Debug.Log($"Mid Term Memory: {string.Join(", ", agentInfo.Memory.MiddleTermMemory)}");
                    Debug.Log($"Long Term Memory: {string.Join(", ", agentInfo.Memory.LongTermMemory)}");

                    Debug.Log($"Behavior: {string.Join(", ", agentInfo.Behavior)}");
                    Debug.Log($"Recent Summary: {agentInfo.Behavior.RecentSummary}");

                    Debug.Log($"Planner: {string.Join(", ", agentInfo.Planner)}");
                    Debug.Log($"Plan: {string.Join(", ", agentInfo.Planner.Plan)}");

                    Debug.Log($"VAD: {string.Join(", ", agentInfo.Vad)}");
                    Debug.Log($"Valence: {string.Join(", ", agentInfo.Vad.Valence)}");
                    Debug.Log($"Arousal: {string.Join(", ", agentInfo.Vad.Arousal)}");
                    Debug.Log($"Dominance: {string.Join(", ", agentInfo.Vad.Dominance)}");
                    foreach (var kv in agentInfo.Vad.Emotion)
                    {
                        Debug.Log($"Emotion: {kv.Key} - {string.Join(", ", kv.Value)}");
                    }
                    Debug.Log($"Mood: {string.Join(", ", agentInfo.Vad.Mood)}");
                    Debug.Log($"Chat: {string.Join(", ", agentInfo.Vad.Chat)}");
                }
                else
                {
                    Debug.LogError("API request failed: " + request.error);
                }
            }
        }

        void OnGUI()
        {
            sectionHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 36,                  // 기존 24 → 36으로 확대
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 6, 12)  // 위/아래 마진 키움
            };

            // 일반 라벨 스타일: 사이즈 18
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,                   // 기존 24 → 18 (원래 의도된 14보다도 크게)
                margin = new RectOffset(0, 0, 4, 4)   // 마진 약간 확대
            };

            // 박스 스타일: 패딩 16, 마진 8
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(16, 16, 16, 16),  // 기존 8 → 16
                margin = new RectOffset(8, 8, 8, 8)       // 기존 4 → 8
            };

            if (m_currentAgent == null)
            {
                GUILayout.Label("No agent loaded.", sectionHeaderStyle);
                return;
            }

            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // ──────────── Agent 기본 정보 ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Agent Info", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", labelStyle, GUILayout.Width(100));
            GUILayout.Label(m_currentAgent.Name, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Age:", labelStyle, GUILayout.Width(100));
            GUILayout.Label(m_currentAgent.Age.ToString(), labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Chat ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Chat", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Model:", labelStyle, GUILayout.Width(100));
            GUILayout.Label(m_currentAgent.Chat.LLM.Model, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Memory ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Memory", sectionHeaderStyle);
            void DrawList(string title, System.Collections.Generic.List<string> list)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(title + ":", labelStyle, GUILayout.Width(100));
                string txt = (list != null && list.Count > 0)
                    ? string.Join(", ", list)
                    : "None";
                GUILayout.Label(txt, labelStyle);
                GUILayout.EndHorizontal();
            }
            DrawList("Working", m_currentAgent.Memory.WorkingMemory);
            DrawList("Mid-term", m_currentAgent.Memory.MiddleTermMemory);
            DrawList("Long-term", m_currentAgent.Memory.LongTermMemory);
            GUILayout.EndVertical();

            // ──────────── Behavior ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Behavior", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Recent:", labelStyle, GUILayout.Width(100));
            GUILayout.Label(string.IsNullOrEmpty(m_currentAgent.Behavior.RecentSummary)
                             ? "None"
                             : m_currentAgent.Behavior.RecentSummary,
                             labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Planner ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Planner", sectionHeaderStyle);
            if (m_currentAgent.Planner.Plan != null && m_currentAgent.Planner.Plan.Count > 0)
            {
                foreach (var kv in m_currentAgent.Planner.Plan)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(kv.Key + ":", labelStyle, GUILayout.Width(100));
                    GUILayout.Label(kv.Value, labelStyle);
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No plan items", labelStyle);
            }
            GUILayout.EndVertical();

            // ──────────── VAD ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("VAD", sectionHeaderStyle);
            void DrawDict<T>(string title, Dictionary<string, T> dict)
            {
                GUILayout.Label(title + ":", labelStyle);
                if (dict != null && dict.Count > 0)
                {
                    foreach (var kv in dict)
                    {
                        string display;
                        // List<string> (or IEnumerable<string>) 면 내부 요소를 쉼표로 join
                        if (kv.Value is IEnumerable<string> stringList)
                            display = string.Join(", ", stringList);
                        else
                            display = kv.Value?.ToString() ?? "None";

                        GUILayout.Label($"  {kv.Key}: {display}", labelStyle);
                    }
                }
                else
                {
                    GUILayout.Label("  None", labelStyle);
                }
            }
            DrawDict("Valence", m_currentAgent.Vad.Valence);
            DrawDict("Arousal", m_currentAgent.Vad.Arousal);
            DrawDict("Dominance", m_currentAgent.Vad.Dominance);
            DrawDict("Emotion", m_currentAgent.Vad.Emotion);
            DrawDict("Mood", m_currentAgent.Vad.Mood);

            GUILayout.Space(8);
            GUILayout.Label("VAD.Chat", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Model:", labelStyle, GUILayout.Width(100));
            GUILayout.Label(m_currentAgent.Vad.Chat.LLM.Model, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

    }
}