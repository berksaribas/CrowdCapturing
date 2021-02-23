using System;
using Newtonsoft.Json;

namespace JSONDataClasses
{
    public class ConnectionData : IComparable<ConnectionData>
    {
        [JsonProperty("alias")]
        public string BuildingAlias;
        [JsonProperty("start_time")]
        public string StartTime;
        [JsonProperty("end_time")]
        public string EndTime;
        [JsonProperty("groups_with")]
        public int[] GroupsWith;
        
        public int CompareTo(ConnectionData other) => StartTime.CompareTo(other.StartTime);
    }
}