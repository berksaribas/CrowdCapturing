using System;
using System.Collections.Generic;
using Simulation;
using UnityEngine;
using UnityEngine.AI;

namespace World
{
    public class Door : MonoBehaviour
    {
        private const int Capacity = 3;
        private const double EnterDurationInSeconds = 0.5;
        private readonly double[] lastEnterTimes = new double[Capacity];

        public readonly Queue<Agent> WaitingAgents = new Queue<Agent>(16);

        [NonSerialized]
        public Vector3 NavMeshPosition;

        private void Awake()
        {
            if (NavMesh.SamplePosition(transform.position, out var closestHit, 100f, NavMesh.AllAreas))
                NavMeshPosition = closestHit.position;
            else
                Debug.LogAssertion($"The Door \"{gameObject.name}\" has no NavMesh position available!!");
        }

        private void LateUpdate()
        {
            var currentTime = Simulation.SimulationController.Instance.TimeManager.Time;

            for (var i = 0; i < lastEnterTimes.Length && WaitingAgents.Count != 0; i++)
                if (lastEnterTimes[i] + EnterDurationInSeconds <= currentTime)
                {
                    lastEnterTimes[i] = currentTime;
                    WaitingAgents.Dequeue().PassTheDoor();
                }
        }
    }
}