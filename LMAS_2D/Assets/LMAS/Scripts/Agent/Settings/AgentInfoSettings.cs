using System;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
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
}