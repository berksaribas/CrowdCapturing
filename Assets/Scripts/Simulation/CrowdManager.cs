using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using World;

namespace Simulation
{
    public class CrowdManager : MonoBehaviour
    {
        public GameObject AgentPrefab;

        private List<Agent> agents;

        private void Awake()
        {
            agents = new List<Agent>();
        }

        public void CreateAgent(Door startingDoor, Door finishingDoor, MaterialPropertyBlock actorMaterialProperty)
        {
            var agent = Instantiate(AgentPrefab, transform);
            agent.GetComponent<NavMeshAgent>().enabled = false;
            var agentComponent = agent.AddComponent<Agent>();

            agentComponent.SetStartingPosition(startingDoor);
            agentComponent.SetTarget(finishingDoor);

            agent.GetComponent<MeshRenderer>().SetPropertyBlock(actorMaterialProperty);

            agents.Add(agentComponent);
        }

        private void Update()
        {
            foreach (var agent in agents)
            {
                var navMeshAgent = agent.GetComponent<NavMeshAgent>();
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 1f)
                {
                    agent.TargetReached = true;
                }
            }

            var reachedAgents = agents.FindAll(item => item.TargetReached);
            foreach (var reachedAgent in reachedAgents)
            {
                agents.Remove(reachedAgent);
                Destroy(reachedAgent.gameObject);
            }
        }
    }
}