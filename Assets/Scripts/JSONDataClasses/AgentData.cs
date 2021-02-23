using Newtonsoft.Json;

namespace JSONDataClasses
{
    public class AgentData
    {
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("connections")]
        public ConnectionData[] Connections;
    }
}