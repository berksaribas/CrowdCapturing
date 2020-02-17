using UnityEngine;
using Util;

namespace Simulation
{
    public class SimulationTime : MonoBehaviour
    {
        public float Speed = 10f;
        public float TimeInSeconds = 0;

        private float lastRecordedTime = 0f;

        private void Start()
        {
            lastRecordedTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            TimeInSeconds += (Time.realtimeSinceStartup - lastRecordedTime) * Speed;
            lastRecordedTime = Time.realtimeSinceStartup;
        }
    }
}