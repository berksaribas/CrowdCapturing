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
		private List<Agent> activeAgents;
		private Dictionary<Door, Queue<AgentData>> doorAgentQueue;
		private Dictionary<int, Agent> agentIdMap;

		public List<Agent> GetAgents()
		{
			return activeAgents;
		}

		private void Awake()
		{
			agents = new List<Agent>();
			activeAgents = new List<Agent>();
			agentIdMap = new Dictionary<int, Agent>();
			doorAgentQueue = new Dictionary<Door, Queue<AgentData>>();
		}

		public Agent GetAgentById(int agentId)
		{
			return agentIdMap.ContainsKey(agentId) ? agentIdMap[agentId] : null;
		}

		public void GenerateAgents(Dictionary<int, List<Sequence>> agents)
		{
			foreach (var keyValuePair in agents)
			{
				var agent = Instantiate(AgentPrefab, transform);
				agent.GetComponent<NavMeshAgent>().enabled = false;
				var agentComponent = agent.AddComponent<Agent>();
				agentComponent.SetAgentId(keyValuePair.Key);
				agentComponent.SetSequences(keyValuePair.Value);
				agentIdMap.Add(keyValuePair.Key, agentComponent);
				this.agents.Add(agentComponent);
				
				agent.SetActive(false);
			}
		}

		public void ActivateAgent(int agentId, MaterialPropertyBlock actorMaterialProperty)
		{
			var agent = GetAgentById(agentId);
			var sequence = agent.GetNextSequence();

			var startingDoor =
				sequence.StartingBuilding.GetDoorByTargetBuilding(sequence.TargetBuilding);

			if (!doorAgentQueue.ContainsKey(startingDoor))
			{
				doorAgentQueue.Add(startingDoor, new Queue<AgentData>());
			}

			doorAgentQueue[startingDoor].Enqueue(new AgentData(agent, actorMaterialProperty, startingDoor));
		}

		private void ProcessAgentCreateQueue()
		{
			foreach (var keyValuePair in doorAgentQueue)
			{
				while (keyValuePair.Key.IsDoorAvailable() && keyValuePair.Value.Any())
				{
					var agentData = keyValuePair.Value.Dequeue();
					var agent = agentData.Agent;

					if (agentData.IsCreateData)
					{
						agent.gameObject.SetActive(true);
						var sequence = agent.GetNextSequence();

						agent.gameObject.GetComponent<NavMeshAgent>().enabled = false;

						var startingDoor = agentData.StartingDoor;
						
						var targetDoor =
							sequence.StartingBuilding.GetFinishingDoorByTargetBuilding(startingDoor,
								sequence.TargetBuilding);

						agent.SetStartingPosition(startingDoor);
						agent.SetTarget(targetDoor);

						agent.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(agentData.MaterialPropertyBlock);

						agent.StartSequence();

						activeAgents.Add(agent);
					}
					else
					{
						agent.EndSequence();
						agent.gameObject.SetActive(false);

						activeAgents.Remove(agent);
					}
				}
			}
		}

		private void Update()
		{
			foreach (var agent in activeAgents)
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