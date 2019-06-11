using UnityEngine;
using Util;

namespace Simulation
{
    public class SimulationManager : MonoBehaviour
    {
        public float WorldSpeed = 10f;
        public float WorldTimeSeconds = 0;

        private float lastRecordedTime = 0f;

        private void Start()
        {
            lastRecordedTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            WorldTimeSeconds += (Time.realtimeSinceStartup - lastRecordedTime) * WorldSpeed;
            lastRecordedTime = Time.realtimeSinceStartup;
        }
    }
}