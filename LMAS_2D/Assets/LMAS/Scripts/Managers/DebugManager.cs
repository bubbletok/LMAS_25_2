using System.Collections.Generic;

using LMAS.Scripts.Agent.Settings;

namespace LMAS.Scripts.Manager
{
    public class DebugManager : MonoSingleton<DebugManager>
    {
        // This class is used to manage the debug settings for the agent
        // It contains references to the agent's chat, behavior, and planner settings
        // These settings can be modified in the Unity Inspector

        protected Dictionary<string, AgentInfo> m_AgentDictionary = new Dictionary<string, AgentInfo>();

        protected override void OnDestroy()
        {
            m_AgentDictionary.Clear();
            m_AgentDictionary = null;

            base.OnDestroy();
        }

        public AgentInfo GetAgentInfo(string agentName)
        {
            if (m_AgentDictionary.ContainsKey(agentName))
            {
                return m_AgentDictionary[agentName];
            }
            return null;
        }

        public void AddAgent(string agentName, AgentInfo agentInfo)
        {
            if (!m_AgentDictionary.ContainsKey(agentName))
            {
                m_AgentDictionary.Add(agentName, agentInfo);
            }
        }

        public void RemoveAgent(string agentName)
        {
            if (m_AgentDictionary.ContainsKey(agentName))
            {
                m_AgentDictionary.Remove(agentName);
            }
        }
    }
}