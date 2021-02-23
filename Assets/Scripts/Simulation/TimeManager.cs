using UnityEngine;

namespace Simulation
{
    public class TimeManager : MonoBehaviour
    {
        public int StartHour = 6, StartMinute = 0;

        public float Speed = 10f;
        public float TimeInSeconds { get; private set; }
        public float DeltaTime { get; private set; }
        
        private void Awake()
        {
            TimeInSeconds = (StartHour * 60 + StartMinute) * 60;
        }

        private void Update()
        {
            DeltaTime = Time.deltaTime * Speed;
            TimeInSeconds += DeltaTime;
        }
    }
}