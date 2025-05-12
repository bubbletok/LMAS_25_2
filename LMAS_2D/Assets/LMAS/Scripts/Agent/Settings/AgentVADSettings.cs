using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
    [Serializable]
    public class AgentVAD
    {
        [JsonProperty("valence")]
        public Dictionary<string, float> Valence = new Dictionary<string, float>();
        [JsonProperty("arousal")]
        public Dictionary<string, float> Arousal = new Dictionary<string, float>();
        [JsonProperty("dominance")]
        public Dictionary<string, float> Dominance = new Dictionary<string, float>();
        [JsonProperty("emotion")]
        public Dictionary<string, List<string>> Emotion = new Dictionary<string, List<string>>();
        [JsonProperty("mood")]
        public Dictionary<string, string> Mood = new Dictionary<string, string>();
        [JsonProperty("chat")]
        public AgentChat Chat;
    }
}