using System;
using Newtonsoft.Json;

namespace LMAS.Scripts.Agent.Settings
{
    [Serializable]
    public class LLM
    {
        [JsonProperty("verbose")]
        public bool Verbose;
        [JsonProperty("base_url")]
        public string BaseUrl;
        [JsonProperty("model")]
        public string Model;
        [JsonProperty("disable_streaming")]
        public bool DisableStreaming;

        public override string ToString()
        {
            return $"Verbose: {Verbose}, BaseUrl: {BaseUrl}, Model: {Model}, DisableStreaming: {DisableStreaming}";
        }
    }
}