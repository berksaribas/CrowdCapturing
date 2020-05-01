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
        public AgentState State;

        private NavMeshAgent agent;
        private Door startingDoor, targetDoor;
        private Queue<Sequence> sequences;
        private int agentId;

        public float originalSpeed = 0f;
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.acceleration = 150000f;
            originalSpeed = Random.Range(6f, 11f);
            agent.speed = originalSpeed / 10f * SimulationController.Instance.SimulationTime.Speed;
            agent.angularSpeed = 3600f / 10f * SimulationController.Instance.SimulationTime.Speed;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            agent.autoBraking = true;
            State = AgentState.Idling;
        }

        private void Update()
        {
            agent.speed = originalSpeed / 10f  * SimulationController.Instance.SimulationTime.Speed;
            agent.angularSpeed = 3600f / 10f * SimulationController.Instance.SimulationTime.Speed;
        }

        public void SetSequences(List<Sequence> sequences)
        {
            if (this.sequences == null)
            {
                this.sequences = new Queue<Sequence>();
            }
            
            foreach (var sequence in sequences)
            {
                this.sequences.Enqueue(sequence);
            }
            
            if(sequences.Count > 0)
            {
                SimulationController.Instance.BuildingManager.RegisterAgent(
                    sequences[0].StartingBuildingId,
                    this
                );
            }
        }

        public int GetAgentId() => agentId;
        public void SetAgentId(int id)
        {
            agentId = id;
        }

        public void SetTarget(Door door)
        {
            GetComponent<NavMeshAgent>().enabled = true;

            targetDoor = door;

            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(door.gameObject.transform.position, out closestHit, 100f, NavMesh.AllAreas))
            {
                agent.SetDestination(closestHit.position);
            }

            agent.isStopped = false;
        }

        public void SetTarget(Vector3 targetPosition)
        {
            GetComponent<NavMeshAgent>().enabled = true;

            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(targetPosition, out closestHit, 100f, NavMesh.AllAreas))
            {
                agent.SetDestination(closestHit.position);
            }
            
            agent.isStopped = false;
        }

        public void SetStartingPosition(Door door)
        {
            startingDoor = door;

            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(door.gameObject.transform.position, out closestHit, 100f, NavMesh.AllAreas))
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
            
            GetComponent<Renderer>().enabled = true;
            State = state;
        }

        public void EndSequence()
        {            
            SimulationController.Instance.BuildingManager.RegisterAgent(
                sequences.Dequeue().TargetBuildingId,
                this
            );
            
            GetComponent<Renderer>().enabled = false;
            GetComponent<NavMeshAgent>().enabled= false;
            State = AgentState.Idling;
        }
        
        public float GetSpeed() => originalSpeed;
        public void SetSpeed(float speed)
        {
            originalSpeed = speed;
        }
        
        public Door GetStartingDoor() => startingDoor;
        public string GetStartingDoorName() => startingDoor.name;

        public Door GetTargetDoor() => targetDoor;
        public string GetTargetDoorName() => targetDoor.name;

        public bool HasPath() => agent.hasPath;
        public Vector3[] GetPathCorners() => agent.path.corners;
    }
}