using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<Door, Queue<AgentData>> doorAgentQueue;

        private void Awake()
        {
            agents = new List<Agent>();
            doorAgentQueue = new Dictionary<Door, Queue<AgentData>>();
        }

        public void CreateAgent(Door startingDoor, Door finishingDoor, MaterialPropertyBlock actorMaterialProperty)
        {
            AgentData agent = new AgentData(startingDoor, finishingDoor, actorMaterialProperty);
            
            if (!doorAgentQueue.ContainsKey(startingDoor))
            {
                doorAgentQueue.Add(startingDoor, new Queue<AgentData>());
            }
            
            doorAgentQueue[startingDoor].Enqueue(agent);
        }

        private void ProcessAgentCreateQueue()
        {
            foreach (var keyValuePair in doorAgentQueue)
            {
                while (keyValuePair.Key.IsDoorAvailable() && keyValuePair.Value.Any())
                {
                    var agentData = keyValuePair.Value.Dequeue();
                    
                    if(agentData.IsCreateData)
                    {
                        var agent = Instantiate(AgentPrefab, transform);
                        agent.GetComponent<NavMeshAgent>().enabled = false;

                        var agentComponent = agent.AddComponent<Agent>();
                        agentComponent.SetStartingPosition(agentData.StartingDoor);
                        agentComponent.SetTarget(agentData.TargetDoor);

                        agent.GetComponent<MeshRenderer>().SetPropertyBlock(agentData.MaterialPropertyBlock);
                        agents.Add(agentComponent);
                    }
                    else
                    {
                        agents.Remove(agentData.agent);
                        Destroy(agentData.agent.gameObject);
                    }
                }
            }
        }
        
        private void Update()
        {
            foreach (var agent in agents)
            {
                var navMeshAgent = agent.GetComponent<NavMeshAgent>();
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 1f && !agent.TargetReached)
                {
                    agent.TargetReached = true;
                    
                    if (!doorAgentQueue.ContainsKey(agent.GetTargetDoor()))
                    {
                        doorAgentQueue.Add(agent.GetTargetDoor(), new Queue<AgentData>());
                    }
            
                    doorAgentQueue[agent.GetTargetDoor()].Enqueue(new AgentData(agent));
                }
            }
            
            ProcessAgentCreateQueue();
        }
    }
}