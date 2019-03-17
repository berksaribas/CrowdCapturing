using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using World;

namespace Simulation
{
    public class Agent : MonoBehaviour
    {
        public bool TargetReached = false;
        
        private NavMeshAgent agent;
        private Door startingDoor, targetDoor;
        private Queue<Sequence> sequences;
        private int agentId;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.autoBraking = true;
            agent.acceleration = 1000f;
            agent.speed = Random.Range(6f, 11f);
            agent.angularSpeed = 3600f;
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

        public void StartSequence()
        {
            var sequence = GetNextSequence();
            
            sequence.StartingBuilding.UnregisterAgent(this);
            GetComponent<Renderer>().enabled = true;
            TargetReached = false;
        }

        public void EndSequence()
        {
            var sequence = sequences.Dequeue();
            
            sequence.TargetBuilding.RegisterAgent(this);
            GetComponent<Renderer>().enabled = false;
            GetComponent<NavMeshAgent>().enabled= false;
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
    }
}