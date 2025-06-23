using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;

using LMAS.Scripts.Manager;
using LMAS.Scripts.Agent.Settings;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;

using LMAS.Scripts.Types;

namespace LMAS.Scripts.Agent
{
    public class LLMAgent : LMASObject
    {
        #region Properties
        [Header("Agent Settings")]
        [SerializeField] private string m_AgentName;
        [HideInInspector] public string AgentName => m_AgentName;
        private AgentInfo m_AgentInfo;
        [HideInInspector] public AgentInfo AgentInfo => m_AgentInfo;
        private LLMAgentMemory m_AgentMemory;
        [HideInInspector] public LLMAgentMemory AgentMemory => m_AgentMemory;
        private LLMAgentBehavior m_AgentBehavior;
        [HideInInspector] public LLMAgentBehavior AgentBehavior => m_AgentBehavior;
        private LLMAgentPlanner m_AgentPlanner;
        [HideInInspector] public LLMAgentPlanner AgentPlanner => m_AgentPlanner;
        private LLMAgentVAD m_AgentVAD;
        [HideInInspector] public LLMAgentVAD AgentVAD => m_AgentVAD;
        private LLMAgentInteractor m_AgentInteractor;
        [HideInInspector] public LLMAgentInteractor AgentInteractor => m_AgentInteractor;

        private Vector3 m_AgentWorldPos;
        public Vector3 AgentWorldPos => m_AgentWorldPos;
        private Vector3Int m_AgentTilePos;
        public Vector3Int AgentTilePos => m_AgentTilePos;

        private bool m_CanDoAction = false;
        public bool CanDoAction => m_CanDoAction;

        // TEST
        private LLMAgentController m_AgentController;
        #endregion

        #region Unity Methods
        void Awake()
        {
            Type = LMASType.Agent;

            AddAgentComponents();
            StartCoroutine(LoadAgentInfo());

            // TEST
            m_AgentController.Agent = this;
        }

        void Start()
        {
            LocateAgentToTilemap();
            SimulationManager.Instance.OnSimulationStep.AddListener(SimulationStep);
        }

        void Update()
        {
            UpdateAgentPosition();
            HandleInput();
        }

        void OnDestroy()
        {
            DebugManager.Instance.RemoveAgent(m_AgentName);
        }

        #endregion

        private void AddAgentComponents()
        {
            m_AgentMemory = gameObject.AddComponent<LLMAgentMemory>();
            m_AgentBehavior = gameObject.AddComponent<LLMAgentBehavior>();
            m_AgentPlanner = gameObject.AddComponent<LLMAgentPlanner>();
            m_AgentVAD = gameObject.AddComponent<LLMAgentVAD>();
            m_AgentInteractor = gameObject.AddComponent<LLMAgentInteractor>();

            // AgentController must be added last to ensure it can access all other components
            m_AgentController = gameObject.AddComponent<LLMAgentController>();
        }

        private IEnumerator LoadAgentInfo()
        {
            m_CanDoAction = false;
            if (string.IsNullOrWhiteSpace(m_AgentName))
            {
                Debug.LogWarning("Please enter a valid agent name.");
                yield break;
            }
            int curTry = 0;
            const int MAX_TRY = 10;
            const float WAIT_TIME = 3f;
            bool isAgentInfoLoaded = false;
            do
            {
                curTry++;
                yield return StartCoroutine(LoadAgentInfoCoroutine(m_AgentName, (success) => isAgentInfoLoaded = success));
                if (!isAgentInfoLoaded) yield return new WaitForSeconds(WAIT_TIME); // Retry after a short delay
            }
            while (!isAgentInfoLoaded && curTry < MAX_TRY);

            if (!isAgentInfoLoaded)
            {
                Debug.LogError($"Failed to load agent info after {MAX_TRY} attempts.");
                yield break;
            }

            Debug.Log($"Agent {m_AgentName} info is loaded successfully.");
            m_CanDoAction = true;
        }

        private IEnumerator LoadAgentInfoCoroutine(string agentName, UnityAction<bool> callback = null)
        {
            bool isSucceed = false;
            string requestUrl = APISetting.APIUrl + $"/agent/{agentName}";
            Debug.Log("Requesting agent info from: " + requestUrl);
            string debug = "";
            using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AgentInfo agentInfo = JsonConvert.DeserializeObject<AgentInfo>(request.downloadHandler.text);
                    m_AgentInfo = agentInfo; // Store the current agent info

                    debug += $"Agent Name: {agentInfo.Name} Age: {agentInfo.Age}\n";
                    debug += $"Chat: {agentInfo.Chat}\n";
                    debug += $"LLM: {agentInfo.Chat.LLM}\n";
                    debug += $"Memory: {string.Join(", ", agentInfo.Memory)}\n";
                    string FormatMemoryList(string label, List<MemoryData> memoryList)
                    {
                        if (memoryList == null || memoryList.Count == 0)
                            return $"{label}:\n- None\n";

                        var lines = new List<string> { $"{label}:" };
                        foreach (var memory in memoryList)
                        {
                            if (memory == null || memory.Metadata == null) continue;

                            string contentPreview = memory.PageContent.Length > 100
                                ? memory.PageContent.Substring(0, 100) + "..."
                                : memory.PageContent;

                            lines.Add($"- [Type: {memory.Type}]");
                            lines.Add($"  Content: {contentPreview}");
                            lines.Add($"  Importance: {memory.Metadata.Importance:F3}, Time: {memory.Metadata.time:F3}");
                        }
                        return string.Join("\n", lines) + "\n";
                    }
                    debug += FormatMemoryList("Working Memory", agentInfo.Memory.WorkingMemory);
                    debug += FormatMemoryList("Mid-Term Memory", agentInfo.Memory.MiddleTermMemory);
                    debug += FormatMemoryList("Long-Term Memory", agentInfo.Memory.LongTermMemory);
                    debug += $"Recent Summary: {agentInfo.Memory.RecentSummary}\n";
                    debug += $"Behavior: {string.Join(", ", agentInfo.Behavior)}\n";
                    debug += $"Chat: {string.Join(", ", agentInfo.Behavior.Chat)}\n";
                    debug += $"Planner: {string.Join(", ", agentInfo.Planner)}\n";
                    debug += $"Plan: {string.Join(", ", agentInfo.Planner.Plan)}\n";
                    debug += $"VAD: {string.Join(", ", agentInfo.Vad)}\n";
                    debug += $"Valence: {string.Join(", ", agentInfo.Vad.Valence)}\n";
                    debug += $"Arousal: {string.Join(", ", agentInfo.Vad.Arousal)}\n";
                    debug += $"Dominance: {string.Join(", ", agentInfo.Vad.Dominance)}\n";
                    foreach (var kv in agentInfo.Vad.Emotion)
                    {
                        debug += $"Emotion: {kv.Key} - {string.Join(", ", kv.Value)}\n";
                    }
                    debug += $"Mood: {string.Join(", ", agentInfo.Vad.Mood)}\n";
                    debug += $"VAD Chat: {string.Join(", ", agentInfo.Vad.Chat)}\n";
                    Debug.Log(debug);

                    DebugManager.Instance.UpdateAgent(agentName, agentInfo);
                    isSucceed = true;
                }
                else
                {
                    Debug.LogError("API request failed: " + request.error);
                    isSucceed = false;
                }

                callback?.Invoke(isSucceed);
            }
        }

        public void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(LoadAgentInfo());
            }
        }

        // Locate the agent adjust to the tilemap
        public void LocateAgentToTilemap()
        {
            UpdateAgentPosition();
            transform.position = m_AgentWorldPos;
        }

        public void UpdateAgentPosition()
        {
            m_AgentWorldPos = TilemapManager.Instance.GetWorldPosOnTilemap(transform.position);

            m_AgentTilePos = TilemapManager.Instance.GetPosOnTilemap(m_AgentWorldPos);
            if (m_AgentTilePos == Vector3.negativeInfinity)
            {
                Debug.LogError("Agent position is not on any tilemap. Please check the tilemap configuration.");
                return;
            }
        }

        // Perform the simulation step logic here
        // 1. Tilemap 기반 환경 만들기
        // 2. Tilemap observe하는거 만들기
        // 3. 현재 observe한거 + 기억에 있는 것들 합쳐서 행동하기
        // 4. observe한거 + 현재 한 행동 기억에 저장하기

        // 5. 시간이 지남에 따라 working -> mid term -> long term 기억으로 전환하기

        // if Agent has a plan
        // 1. Observe the environment
        // 2. Generate action based on the plan, observation
        // 3. Execute the action


        // if Agent has no plan
        // 1. Observe the environment
        // 2. Generate emotional response based on the observation
        // 3. Generate action based on the emotional response
        // 4. Execute the action
        // 5. Generate plan based on the action, or go back to step 1
        void SimulationStep(float deltaTime)
        {

            Act(deltaTime);
        }

        public void Observe(string observation = "", float deltaTime = 0.0f)
        {
            if (!m_CanDoAction)
            {
                // Debug.LogWarning("Agent cannot perform observations yet. Please wait until the agent info is loaded.");
                return;
            }
            m_CanDoAction = false;

            m_AgentController.Observe(observation, deltaTime, (string result) =>
            {
                m_CanDoAction = true;
            });
        }

        public void Act(float deltaTime)
        {
            if (!m_CanDoAction)
            {
                // Debug.LogWarning("Agent cannot perform actions yet. Please wait until the agent info is loaded.");
                return;
            }
            m_CanDoAction = false;

            m_AgentController.Act(deltaTime, () =>
            {
                string observation = m_AgentController.GetObservation();
                m_AgentController.Observe(observation, deltaTime, (string result) =>
                {
                    OnActionCompleted();
                });

            });

        }

        public void OnActionCompleted()
        {
            Debug.Log($"Action completed for agent: {m_AgentName}");
            m_CanDoAction = true;
        }

        public bool IsReadyToAct()
        {
            return m_CanDoAction;
        }
    }
}
