using UnityEngine;
using LMAS.Scripts.Agent.Settings;
using System.Collections.Generic;
using LMAS.Scripts.Manager;

namespace LMAS.Scripts.Agent.Debugger
{
    public class AgentInfoDebugger : MonoBehaviour
    {
        [SerializeField] private string m_AgentName;
        private Vector2 m_ScrollPos = Vector2.zero;
        // Memory 전용 스크롤 위치
        private Vector2 m_MemoryScrollPos = Vector2.zero;
        // GUIStyles
        GUIStyle sectionHeaderStyle;
        GUIStyle labelStyle;
        GUIStyle boxStyle;

        float m_LabelWidth = 300f; // 라벨의 기본 너비

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

            if (string.IsNullOrEmpty(m_AgentName))
            {
                GUILayout.Label("Agent Name not set.", sectionHeaderStyle);
                return;
            }

            if (DebugManager.Instance == null) return;

            AgentInfo m_currentAgent = DebugManager.Instance.GetAgentInfo(m_AgentName);

            if (m_currentAgent == null)
            {
                GUILayout.Label($"Agent {m_AgentName} not found.", sectionHeaderStyle);
                return;
            }

            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // ──────────── Agent 기본 정보 ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Agent Info", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(m_currentAgent.Name, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Age:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(m_currentAgent.Age.ToString(), labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Chat ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Chat", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Model:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(m_currentAgent.Chat.LLM.Model, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Memory ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Memory", sectionHeaderStyle);

            float memorySectionHeight = 100f; // 필요에 따라 조정 가능
            m_MemoryScrollPos = GUILayout.BeginScrollView(
                m_MemoryScrollPos,
                GUILayout.Height(memorySectionHeight)
            );
            DrawMemoryList("Working", m_currentAgent.Memory.WorkingMemory);
            DrawMemoryList("Mid-term", m_currentAgent.Memory.MiddleTermMemory);
            DrawMemoryList("Long-term", m_currentAgent.Memory.LongTermMemory);
            GUILayout.EndScrollView(); // ▶ Memory 전용 스크롤뷰 끝

            GUILayout.Label("Recent Summary:", labelStyle, GUILayout.Width(m_LabelWidth));
            if (m_currentAgent.Memory.RecentSummary != null && m_currentAgent.Memory.RecentSummary.Count > 0)
            {
                foreach (var summary in m_currentAgent.Memory.RecentSummary)
                {
                    GUILayout.Label($"- {summary}", labelStyle);
                }
            }
            else
            {
                GUILayout.Label("None", labelStyle);
            }
            GUILayout.Label("Model:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(m_currentAgent.Memory.Chat.LLM.Model, labelStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Consolidate Threshold:", labelStyle, GUILayout.Width(180));
            GUILayout.Label(m_currentAgent.Memory.ConsolidateThreshold.ToString("F2"), labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Importance Score Weight:", labelStyle, GUILayout.Width(180));
            GUILayout.Label(m_currentAgent.Memory.ImportanceScoreWeight.ToString("F2"), labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Behavior ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Behavior", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Recent:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(string.IsNullOrEmpty(m_currentAgent.Behavior.RecentAction)
                             ? "None"
                             : m_currentAgent.Behavior.RecentAction,
                             labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Model:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(m_currentAgent.Behavior.Chat.LLM.Model, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // ──────────── Planner ────────────
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Planner", sectionHeaderStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Plan:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(string.IsNullOrEmpty(m_currentAgent.Planner.Plan)
                             ? "None"
                             : m_currentAgent.Planner.Plan,
                             labelStyle);
            GUILayout.EndHorizontal();
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
            GUILayout.Label("Model:", labelStyle, GUILayout.Width(m_LabelWidth));
            GUILayout.Label(m_currentAgent.Vad.Chat.LLM.Model, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        void DrawMemoryList(string title, List<MemoryData> memoryList)
        {
            GUILayout.Label($"== {title} Memory ==", sectionHeaderStyle);

            if (memoryList == null || memoryList.Count == 0)
            {
                GUILayout.Label("None", labelStyle);
                return;
            }

            foreach (var memory in memoryList)
            {
                if (memory == null || memory.Metadata == null)
                    continue;

                GUILayout.BeginVertical(boxStyle);
                GUILayout.Label($"Content: {memory.PageContent}", labelStyle);
                GUILayout.Label($"Type: {memory.Type}", labelStyle);
                GUILayout.Label($"Importance: {memory.Metadata.Importance:F2}", labelStyle);
                GUILayout.Label($"Time: {memory.Metadata.time:F2}", labelStyle);
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
        }
    }
}