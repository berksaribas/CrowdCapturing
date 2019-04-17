using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
			int i = 0;
			foreach (var keyValuePair in agents)
			{
				if (keyValuePair.Value.Count > 0)
				{
					i++;
				
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
			Debug.Log($"Created agents: {i}");
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

			agent.State = AgentState.WaitingLeavingDoor;

			if (SimulationController.Instance.GroupManager.CanCreateAGroup(agent, sequence) ||
			    SimulationController.Instance.GroupManager.IsMemberOfAGroup(agent))
			{
				doorAgentQueue[startingDoor].Enqueue(new AgentData(agent, actorMaterialProperty, startingDoor,
					AgentData.DataType.IndividualMove));
			}
			else
			{
				doorAgentQueue[startingDoor].Enqueue(new AgentData(agent, actorMaterialProperty, startingDoor,
					AgentData.DataType.GroupMoveBeforeMeet));
			}
			
		}

		private void ProcessAgentCreateQueue()
		{
			foreach (var keyValuePair in doorAgentQueue)
			{
				while (keyValuePair.Key.IsDoorAvailable() && keyValuePair.Value.Any())
				{
					var agentData = keyValuePair.Value.Dequeue();
					var agent = agentData.Agent;

					switch (agentData.Type)
					{
						case AgentData.DataType.IndividualMove:
							ProcessIndividualMoveAgent(agentData);
							activeAgents.Add(agent);
							break;
						case AgentData.DataType.GroupMoveBeforeMeet:
							ProcessGroupMoveBeforeMeet(agentData);
							break;
						case AgentData.DataType.FinishSequence:
							ProcessFinishSequenceAgent(agentData);
							activeAgents.Remove(agent);
							break;
					}
				}
			}
		}

		private void ProcessIndividualMoveAgent(AgentData agentData)
		{
			var agent = agentData.Agent;
			
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

			agent.StartSequence(AgentState.WalkingToTargetDoor);
		}
		
		private void ProcessGroupMoveBeforeMeet(AgentData agentData)
		{
			var agent = agentData.Agent;
			GroupSequence groupSequence = null;

			if (SimulationController.Instance.GroupManager.IsMemberOfAGroup(agent))
			{
				groupSequence = SimulationController.Instance.GroupManager.GetActiveGroup(agent);
			}
			else if(SimulationController.Instance.GroupManager.CanCreateAGroup(agent, agent.GetNextSequence()))
			{
				groupSequence = SimulationController.Instance.GroupManager.CreateGroup(agent, agent.GetNextSequence(), agentData.StartingDoor);
			}
			else
			{
				ProcessIndividualMoveAgent(agentData);
				return;
			}
			
			agent.gameObject.SetActive(true);
			agent.gameObject.GetComponent<NavMeshAgent>().enabled = false;

			agent.SetStartingPosition(agentData.StartingDoor);
			agent.SetTarget(groupSequence.MeetingPoint);

			agent.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(agentData.MaterialPropertyBlock);

			agent.StartSequence(AgentState.WalkingToMeetingPosition);
		}

		private void ProcessFinishSequenceAgent(AgentData agentData)
		{
			var agent = agentData.Agent;

			if (SimulationController.Instance.GroupManager.IsMemberOfAGroup(agent))
			{
				SimulationController.Instance.GroupManager.RemoveFromGroup(agent);
			}

			agent.EndSequence();
			agent.gameObject.SetActive(false);
		}

		private void Update()
		{
			foreach (var agent in activeAgents)
			{
				var navMeshAgent = agent.GetComponent<NavMeshAgent>();
				if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 1f)
				{
					if(agent.State == AgentState.WalkingToTargetDoor)
					{
						agent.State = AgentState.WaitingEnteringDoor;

						if (!doorAgentQueue.ContainsKey(agent.GetTargetDoor()))
						{
							doorAgentQueue.Add(agent.GetTargetDoor(), new Queue<AgentData>());
						}

						doorAgentQueue[agent.GetTargetDoor()].Enqueue(new AgentData(agent));
					}
					else if (agent.State == AgentState.WalkingToMeetingPosition)
					{
						agent.State = AgentState.WaitingGroupMembers;
						SimulationController.Instance.GroupManager.GetActiveGroup(agent).MarkAgentArrived();
					}
				}
			}

			ProcessAgentCreateQueue();
		}
	}
}