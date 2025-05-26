using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
    [Serializable]
    public class Metadata
    {
        [JsonProperty("importance")]
        public float Importance;
        [JsonProperty("time")]
        public float time;
    }

    [Serializable]
    public class MemoryData
    {
        [JsonProperty("metadata")]
        public Metadata Metadata;
        [JsonProperty("page_content")]
        public string PageContent;
        [JsonProperty("type")]
        public string Type;
    }
    [Serializable]
    public class AgentMemory
    {
        [JsonProperty("working_memory")]
        public List<MemoryData> WorkingMemory;
        [JsonProperty("mid_term_memory")]
        public List<MemoryData> MiddleTermMemory;
        [JsonProperty("long_term_memory")]
        public List<MemoryData> LongTermMemory;
        [JsonProperty("chat")]
        public AgentChat Chat;
        [JsonProperty("consolidate_threshold")]
        public float ConsolidateThreshold;
        [JsonProperty("importance_score_weight")]
        public float ImportanceScoreWeight;
    }
}