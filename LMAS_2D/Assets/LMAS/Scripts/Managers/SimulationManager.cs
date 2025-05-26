using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LMAS.Scripts.Agent;

namespace LMAS.Scripts.Manager
{
    public class SimulationManager : MonoSingleton<SimulationManager>
    {
        // Current simulation time in seconds
        private float m_SimulationTime;
        public float SimulationTime => m_SimulationTime;
        [HideInInspector] public UnityEvent<float> OnSimulationStep = new UnityEvent<float>();
        [SerializeField] private bool m_RunByFixedFrame = false;
        private bool m_SimulationStarted = false;
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

        private void Update()
        {
            if (m_RunByFixedFrame) return;

            if (m_SimulationStarted && !m_SimulationPaused)
            {
                // Call the simulation step method with the time since the last frame
                SimulationStep(Time.deltaTime);
            }

            m_SimulationTime += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (!m_RunByFixedFrame) return;

            if (m_SimulationStarted && !m_SimulationPaused)
            {
                // Call the simulation step method with the fixed delta time
                SimulationStep(Time.fixedDeltaTime);
            }

            m_SimulationTime += Time.fixedDeltaTime;
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
