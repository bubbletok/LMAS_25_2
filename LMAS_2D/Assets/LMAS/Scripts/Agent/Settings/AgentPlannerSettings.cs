using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
    [Serializable]
    public class AgentPlanner
    {
        [JsonProperty("plan")]
        public Dictionary<string, string> Plan;
    }
}