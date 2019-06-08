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
            agent.autoBraking = true;
            agent.acceleration = 1000f;
            originalSpeed = Random.Range(6f, 11f);
            agent.speed = originalSpeed / 10f * SimulationController.Instance.SimulationManager.WorldSpeed;
            agent.angularSpeed = 3600f / 10f * SimulationController.Instance.SimulationManager.WorldSpeed;
            State = AgentState.Idling;
        }

        private void Update()
        {
            agent.speed = originalSpeed / 10f  * SimulationController.Instance.SimulationManager.WorldSpeed;
            agent.angularSpeed = 3600f / 10f * SimulationController.Instance.SimulationManager.WorldSpeed;
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
                sequences[0].StartingBuilding.RegisterAgent(this);
            }
        }

        public void SetAgentId(int agentId)
        {
            this.agentId = agentId;
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
            var sequence = GetNextSequence();
            
            sequence.StartingBuilding.UnregisterAgent(this);
            GetComponent<Renderer>().enabled = true;
            State = state;
        }

        public void EndSequence()
        {
            var sequence = sequences.Dequeue();
            
            sequence.TargetBuilding.RegisterAgent(this);
            GetComponent<Renderer>().enabled = false;
            GetComponent<NavMeshAgent>().enabled= false;
            State = AgentState.Idling;
        }
        
        public float GetSpeed()
        {
            return originalSpeed;
        }

        public void SetSpeed(float speed)
        {
            originalSpeed = speed;
        }

        public int GetAgentId()
        {
            return agentId;
        }
        
        public string GetStartingDoorName()
        {
            return startingDoor.gameObject.name;
        }

        public string GetTargetDoorName()
        {
            return targetDoor.gameObject.name;
        }

        public Door GetStartingDoor()
        {
            return startingDoor;
        }
        
        public Door GetTargetDoor()
        {
            return targetDoor;
        }

        public bool HasPath()
        {
            return agent.hasPath;
        }
        
        public Vector3[] GetPathCorners()
        {
            return agent.path.corners;
        }
    }
}