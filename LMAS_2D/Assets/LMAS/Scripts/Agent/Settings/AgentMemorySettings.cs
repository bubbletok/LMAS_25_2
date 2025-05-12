using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
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
}