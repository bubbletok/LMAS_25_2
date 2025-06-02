using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
    [Serializable]
    public class AgentBehavior
    {
        [JsonProperty("recent_action")]
        public string RecentAction;
        [JsonProperty("chat")]
        public AgentChat Chat;
    }
}