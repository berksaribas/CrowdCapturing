using System;
using Newtonsoft.Json;

namespace DefaultNamespace
{
    public class SequenceData : IComparable<SequenceData>
    {
        [JsonProperty("alias")]
        public string alias;
        [JsonProperty("start_date")]
        public string startDate;
        [JsonProperty("end_date")]
        public string endDate;
        
        public int CompareTo(SequenceData other)
        {
            return startDate.CompareTo(other.startDate);
        }
    }
}