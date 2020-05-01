using UnityEngine;

namespace World
{
    public class Door : MonoBehaviour
    {
        public int Capacity = 3;
        public float EnterDurationInSeconds = 1.5f;

        public float[] LastEnterTimes;

        private void Awake()
        {
            Capacity = 3;
            EnterDurationInSeconds = 0.5f;

            LastEnterTimes = new float[Capacity];

            for (var i = 0; i < LastEnterTimes.Length; i++)
            {
                LastEnterTimes[i] = 0f;
            }
        }

        public bool IsDoorAvailable()
        {
            var currentTime = SimulationController.Instance.SimulationTime.TimeInSeconds;

            for (var i = 0; i < LastEnterTimes.Length; i++)
            {
                if (LastEnterTimes[i] + EnterDurationInSeconds <= currentTime)
                {
                    LastEnterTimes[i] = currentTime;
                    return true;
                }
            }

            return false;
        }
    }
}