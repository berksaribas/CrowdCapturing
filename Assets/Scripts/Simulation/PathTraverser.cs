using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Simulation
{
    [RequireComponent(typeof(Agent))]
    public class PathTraverser : MonoBehaviour
    {
        public float HeightOffset = 1f;
        public Vector3[] Corners;
        private int nextCornerIndex;
        private float remainingDistance;
        private Vector3 currentPathPosition;
        public bool IsArrived => remainingDistance < 1f;

        private Agent agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
        }

        private void Update()
        {
            var distance = agent.Speed * SimulationController.Instance.TimeManager.DeltaTime;
            Traverse(distance);
            
            transform.position = currentPathPosition + Vector3.up * HeightOffset;

            if (IsArrived)
            {
                enabled = false;
                agent.PathTraverseFinished();
            }
        }

        private void Traverse(float distance)
        {
            remainingDistance -= distance;

            if (IsArrived)
            {
                currentPathPosition = Corners.Last();
                return;
            }

            var toCorner = Corners[nextCornerIndex] - currentPathPosition;
            var distToCorner = toCorner.magnitude;

            while (distance > distToCorner)
            {
                currentPathPosition = Corners[nextCornerIndex];
                distance -= distToCorner;
                nextCornerIndex++;

                toCorner = Corners[nextCornerIndex] - currentPathPosition;
                distToCorner = toCorner.magnitude;
            }

            currentPathPosition += toCorner.normalized * distance;
        }
        
        public void SetTarget(Vector3 position)
        {
            if (!NavMesh.SamplePosition(position, out var closestHit, 100f, NavMesh.AllAreas))
            {
                Debug.LogWarning("No NavMesh position found for Agent when setting target position");
                return;
            }

            currentPathPosition = transform.position;
            var targetPosition = closestHit.position;
            
            var navMeshPath = new NavMeshPath();
            if (!NavMesh.CalculatePath(currentPathPosition, targetPosition, NavMesh.AllAreas, navMeshPath))
            {
                Debug.LogWarning($"No NavMeshPATH found for Agent when setting target position");
                return;
            }
            
            SetPath(navMeshPath);
            enabled = true;
        }
        
        public void SetPath(NavMeshPath navMeshPath)
        {
            nextCornerIndex = 1;
            Corners = navMeshPath.corners;

            for (var i = 1; i < Corners.Length; i++)
                remainingDistance += Vector3.Distance(Corners[i - 1], Corners[i]);
        }
    }
}