using System;
using JSONDataClasses;
using UnityEngine;

namespace Simulation
{
    public class TimeManager : MonoBehaviour
    {
        //public int StartHour = 6, StartMinute = 0;

        public bool Playing = false;
        public float Speed = 10f;

        public double Time { get; private set; }
        public float DeltaTime { get; private set; }

        [NonSerialized]
        public double DataRangeStart, DataRangeEnd;

        public void Initialize(AgentData[] agentsData)
        {
            var start = TimeSpan.MaxValue;
            var end = TimeSpan.MinValue;

            foreach (var agent in agentsData)
            foreach (var connection in agent.Connections)
            {
                if (connection.StartTime < start)
                    start = connection.StartTime;

                if (connection.EndTime > end)
                    end = connection.EndTime;
            }

            DataRangeStart = start.TotalSeconds;
            DataRangeEnd = end.TotalSeconds;

            Time = DataRangeStart;
        }

        private void Update()
        {
            if (Playing)
                DeltaTime = UnityEngine.Time.deltaTime * Speed;
            else
                DeltaTime = 0;

            Time += DeltaTime;
        }
    }
}