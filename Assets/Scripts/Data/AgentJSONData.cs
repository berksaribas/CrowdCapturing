using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace
{
    public class AgentJSONData
    {
        [JsonProperty("device_id")]
        public string deviceId;
        [JsonProperty("sequences")]
        public List<SequenceData> sequences;
    }
}