using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using World;

namespace Simulation
{
    public enum AgentState
    {
        Idling,
        WaitingEnteringDoor,
        WaitingLeavingDoor,
        WaitingGroupMembers,
        WalkingToTargetDoor,
        WalkingToMeetingPosition
    }
    
    public class Agent : MonoBehaviour
    {
        //    TODO: These fields should be private
        [HideInInspector] public Renderer MeshRenderer;
        [HideInInspector] public NavMeshAgent NavMeshAgent;

        public int Id;
        public AgentState State;
        private Door startingDoor, targetDoor;
        private Queue<Sequence> sequences;
        private float originalSpeed;
        
        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshAgent.acceleration = 150000f;
            originalSpeed = Random.Range(6f, 11f);
            NavMeshAgent.speed = originalSpeed / 10f * SimulationController.Instance.SimulationTime.Speed;
            NavMeshAgent.angularSpeed = 3600f / 10f * SimulationController.Instance.SimulationTime.Speed;
            NavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            NavMeshAgent.autoBraking = true;
            
            State = AgentState.Idling;
            sequences = new Queue<Sequence>();
        }

        private void Update()
        {
            NavMeshAgent.speed = originalSpeed / 10f  * SimulationController.Instance.SimulationTime.Speed;
            NavMeshAgent.angularSpeed = 3600f / 10f * SimulationController.Instance.SimulationTime.Speed;
        }

        public void InitializeSequences(List<Sequence> initialSequences)
        {
            foreach (var sequence in initialSequences)
            {
                sequences.Enqueue(sequence);
            }
            
            if(initialSequences.Count > 0)
            {
                SimulationController.Instance.BuildingManager.RegisterAgent(
                    sequences.Peek().StartingBuildingId,
                    this
                );
            }
        }

        public void SetTarget(Door door)
        {
            NavMeshAgent.enabled = true;
            targetDoor = door;

            if (NavMesh.SamplePosition(door.transform.position, out var closestHit, 100f, NavMeshAgent.areaMask))
            {
                NavMeshAgent.SetDestination(closestHit.position);
            }

            NavMeshAgent.isStopped = false;
        }

        public void SetTarget(Vector3 targetPosition)
        {
            NavMeshAgent.enabled = true;

            if (NavMesh.SamplePosition(targetPosition, out var closestHit, 100f, NavMeshAgent.areaMask))
            {
                NavMeshAgent.SetDestination(closestHit.position);
            }
            
            NavMeshAgent.isStopped = false;
        }

        public void SetStartingPosition(Door door)
        {
            startingDoor = door;

            if (NavMesh.SamplePosition(door.transform.position, out var closestHit, 100f, NavMeshAgent.areaMask))
            {
                transform.position = closestHit.position;
            }
        }
        
        public Sequence GetNextSequence()
        {
            return sequences.Any() ? sequences.Peek() : null;
        }

        public void StartSequence(AgentState state)
        {
            SimulationController.Instance.BuildingManager.UnregisterAgent(
                GetNextSequence().StartingBuildingId,
                this
            );
            
            MeshRenderer.enabled = true;
            State = state;
        }

        public void EndSequence()
        {            
            SimulationController.Instance.BuildingManager.RegisterAgent(
                sequences.Dequeue().TargetBuildingId,
                this
            );
            
            MeshRenderer.enabled = false;
            NavMeshAgent.enabled= false;
            State = AgentState.Idling;
        }
        
        public float GetSpeed() => originalSpeed;
        public void SetSpeed(float speed)
        {
            originalSpeed = speed;
        }
        
        public Door GetStartingDoor() => startingDoor;

        public Door GetTargetDoor() => targetDoor;
    }
}