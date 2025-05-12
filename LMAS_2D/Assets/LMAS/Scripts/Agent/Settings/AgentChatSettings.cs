using System;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
    [Serializable]
    public class AgentChat
    {
        [JsonProperty("llm")]
        public LLM LLM;

        public override string ToString()
        {
            return $"LLM: {LLM}";
        }
    }
}