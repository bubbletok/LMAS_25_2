using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using LMAS.Scripts.TypedTiles;
using LMAS.Scripts.Types;
using LMAS.Scripts.Agent;
using System.Collections;

namespace LMAS.Scripts.Manager
{
    public class SimulationManager : MonoSingleton<SimulationManager>
    {
        // Current simulation time in seconds
        private float m_SimulationTime;
        public float SimulationTime => m_SimulationTime;
        [HideInInspector] public UnityEvent<float> OnSimulationStep = new UnityEvent<float>();
        [SerializeField] private bool m_RunByFixedFrame = false;
        // private bool m_SimulationStarted = false;
        private bool m_SimulationPaused = true;

        private List<LLMAgent> m_Agents = new List<LLMAgent>();

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            // Initialize the simulation time to zero
            m_SimulationTime = 0f;
            GetAllLLMAgents();
        }

        void Start()
        {
            StartCoroutine(SimulationSetup());
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_SimulationPaused = !m_SimulationPaused;
            }

            if (m_RunByFixedFrame) return;

            if (!IsAllAgentsReady()) return;

            if (!m_SimulationPaused)
            {
                // Call the simulation step method with the time since the last frame
                SimulationStep(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (!m_RunByFixedFrame) return;

            if (!m_SimulationPaused)
            {
                // Call the simulation step method with the fixed delta time
                SimulationStep(Time.fixedDeltaTime);
            }
        }

        protected override void OnDestroy()
        {
            // Clean up the event listeners when the object is destroyed
            OnSimulationStep.RemoveAllListeners();
            m_Agents.Clear();
            m_Agents = null;

            base.OnDestroy();
        }
        #endregion

        private IEnumerator SimulationSetup()
        {
            while (!IsAllAgentsReady())
            {
                // Wait until the TilemapManager is ready
                yield return null;
            }

            ObserveAll();
        }

        private void ObserveAll()
        {
            string observation = "";

            List<(Vector3, TypedTile)> tileList = new List<(Vector3, TypedTile)>();
            foreach (var tileMapData in TilemapManager.Instance.Tilemaps)
            {
                // Get all the tiles
                var tileMap = tileMapData.tilemap;
                var bounds = tileMap.cellBounds;
                foreach (Vector3Int pos in bounds.allPositionsWithin)
                {
                    TileBase tile = tileMap.GetTile(pos);
                    TypedTile typedTile = tile as TypedTile;
                    if (typedTile != null && (typedTile.Type == LMASType.Interactable || typedTile.Type == LMASType.Item))
                    {
                        Vector3 worldPos = tileMap.GetCellCenterWorld(pos);
                        tileList.Add((worldPos, typedTile));
                    }
                }

                if (tileMapData.type == TilemapType.Ground)
                {
                    var minBounds = tileMap.localBounds.min;
                    var maxBounds = tileMap.localBounds.max;
                    var upperLeft = minBounds.x + ", " + maxBounds.y;
                    var upperRight = maxBounds.x + ", " + maxBounds.y;
                    var lowerLeft = minBounds.x + ", " + minBounds.y;
                    var lowerRight = maxBounds.x + ", " + minBounds.y;
                    observation += $"World Bounds: Upper Left({upperLeft}), Upper Right({upperRight}), " +
                                   $"Lower Left({lowerLeft}), Lower Right({lowerRight})\n";
                }
            }
            foreach (var agent in m_Agents)
            {
                observation += $"Agent: {agent.AgentName}, Position: {agent.transform.position}\n";
            }

            Dictionary<string, List<Vector3>> tilePositions = new Dictionary<string, List<Vector3>>();
            foreach (var agent in m_Agents)
            {
                LLMAgentController agentController = agent.GetComponent<LLMAgentController>();
                if (agentController == null) continue;

                string allTiles = "All things: ";
                foreach ((var tilePos, var typedTile) in tileList)
                {
                    string tileFullName = typedTile.Type + " " + typedTile.TileName;
                    if (!tilePositions.ContainsKey(tileFullName))
                    {
                        tilePositions.Add(tileFullName, new List<Vector3> { tilePos });
                    }
                    else
                    {
                        tilePositions[tileFullName].Add(tilePos);
                    }
                }
                foreach (var tilePos in tilePositions)
                {
                    allTiles += tilePos.Key + " at ";
                    foreach (var pos in tilePos.Value)
                    {
                        allTiles += pos + ", ";
                    }
                }

                observation += allTiles + "\n";

                agentController.Observe(observation, 0.0f, (string result) =>
                {
                    agentController.Plan(0.0f);
                });
            }
        }

        public bool IsAllAgentsReady()
        {
            // Check if all agents are ready to perform actions
            foreach (LLMAgent agent in m_Agents)
            {
                if (!agent.CanDoAction)
                {
                    return false;
                }
            }
            return true;
        }

        public List<LLMAgent> GetAllLLMAgents()
        {
            m_Agents.Clear();
            // Find all LLMAgent components in the scene and add them to the list
            LLMAgent[] agents = FindObjectsByType<LLMAgent>(FindObjectsSortMode.None);
            foreach (LLMAgent agent in agents)
            {
                m_Agents.Add(agent);
            }
            return m_Agents;
        }

        public void SimulationStep(float deltaTime)
        {
            // Update the simulation time by the delta time
            m_SimulationTime += deltaTime;

            OnSimulationStep.Invoke(m_SimulationTime);
        }
    }
}
