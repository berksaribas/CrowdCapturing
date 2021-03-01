using System;
using Newtonsoft.Json;
using UnityEngine;

namespace JSONDataClasses
{
    public class TimespanConverter : JsonConverter<TimeSpan>
    {
        private const string TimeSpanFormatString = @"hh\:mm\:ss";
        
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(TimeSpanFormatString));
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                return TimeSpan.ParseExact((string)reader.Value, TimeSpanFormatString, null);
            }
            catch (FormatException e)
            {
                Debug.LogWarning("Date Parsing Error caused by " + (string)reader.Value);
                throw;
            }
        }
    }

    public class ConnectionData : IComparable<ConnectionData>
    {
        [JsonProperty("alias")]
        public string BuildingAlias;
        
        [JsonConverter(typeof(TimespanConverter))]
        [JsonProperty("start_time")]
        public TimeSpan StartTime;
        
        [JsonConverter(typeof(TimespanConverter))]
        [JsonProperty("end_time")]
        public TimeSpan EndTime;
        
        [JsonProperty("groups_with")]
        public int[] GroupsWith;
        
        public int CompareTo(ConnectionData other) => StartTime.CompareTo(other.StartTime);
    }
}