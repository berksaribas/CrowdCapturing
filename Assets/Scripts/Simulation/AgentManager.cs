using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Util;
using World;

namespace Simulation
{
	public class AgentManager : MonoBehaviour
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

		public void ActivateAgent(int agentId)
		{
			var agent = GetAgentById(agentId);
			var sequence = agent.GetNextSequence();

			var startingDoor = SimulationController.Instance
				.BuildingManager.GetDoorByTargetBuilding(
					sequence.StartingBuildingId,
					sequence.TargetBuildingId
				);

			if (!doorAgentQueue.ContainsKey(startingDoor))
			{
				doorAgentQueue.Add(startingDoor, new Queue<AgentData>());
			}

			agent.State = AgentState.WaitingLeavingDoor;
			
			var actorMaterialProperty = new MaterialPropertyBlock();
            var randomColor = SequenceColorHelper.GetColor(
	            sequence.StartingBuildingId,
	            sequence.TargetBuildingId
	        );
            actorMaterialProperty.SetColor("_Color", randomColor);

			if (SimulationController.Instance.GroupManager.CanCreateAGroup(agent, sequence) ||
			    SimulationController.Instance.GroupManager.IsMemberOfAGroup(agent))
			{
				doorAgentQueue[startingDoor].Enqueue(new AgentData(agent, actorMaterialProperty, startingDoor,
					AgentData.DataType.GroupMoveBeforeMeet));
			}
			else
			{
				doorAgentQueue[startingDoor].Enqueue(new AgentData(agent, actorMaterialProperty, startingDoor,
					AgentData.DataType.IndividualMove));
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
							if (!activeAgents.Contains(agent))
							{
								activeAgents.Add(agent);
							}
							break;
						case AgentData.DataType.GroupMoveBeforeMeet:
							ProcessGroupMoveBeforeMeet(agentData);
							if (!activeAgents.Contains(agent))
							{
								activeAgents.Add(agent);
							}
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
			
			var targetDoor = SimulationController.Instance
				.BuildingManager.GetFinishingDoorByTargetBuilding(
					startingDoor,
					sequence.TargetBuildingId
				);

			agent.SetStartingPosition(startingDoor);
			agent.SetTarget(targetDoor);

			agent.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(agentData.MaterialPropertyBlock);

			agent.StartSequence(AgentState.WalkingToTargetDoor);
			
			//Data only
			OverallData.Instance.AddAgentsLeavingBuilding(1);
		}
		
		private void ProcessGroupMoveBeforeMeet(AgentData agentData)
		{
			var agent = agentData.Agent;
			GroupSequence groupSequence = null;

			if (SimulationController.Instance.GroupManager.IsMemberOfAGroup(agent))
			{
				Debug.Log("Already a member of a group!");
				SimulationController.Instance.GroupManager.GetActiveGroup(agent);
			}
			else if(SimulationController.Instance.GroupManager.CanCreateAGroup(agent, agent.GetNextSequence()))
			{
				Debug.Log("Creating a group!");
				SimulationController.Instance.GroupManager.CreateGroup(agent, agent.GetNextSequence(), agentData.StartingDoor);
			}
			else
			{
				Debug.Log("Act as individual");
				ProcessIndividualMoveAgent(agentData);
				return;
			}

			groupSequence = SimulationController.Instance.GroupManager.GetActiveGroup(agent);

			if (groupSequence.LeaveDoorTogether)
			{
				for (var index = 0; index < groupSequence.agents.Count; index++)
				{
					var groupSequenceAgent = groupSequence.agents[index];

					groupSequenceAgent.gameObject.SetActive(true);
					groupSequenceAgent.gameObject.GetComponent<NavMeshAgent>().enabled = false;
					groupSequenceAgent.SetStartingPosition(agentData.StartingDoor);
					groupSequenceAgent.gameObject.GetComponent<MeshRenderer>()
						.SetPropertyBlock(agentData.MaterialPropertyBlock);
					groupSequenceAgent.StartSequence(AgentState.WalkingToMeetingPosition);
					groupSequenceAgent.GetNextSequence().disabled = true;
					groupSequence.MarkAgentArrived();
					if (!activeAgents.Contains(groupSequenceAgent))
					{
						activeAgents.Add(groupSequenceAgent);
					}
				}
				OverallData.Instance.AddAgentsLeavingBuilding(groupSequence.agents.Count);
			}
			else
			{
				agent.gameObject.SetActive(true);
				agent.gameObject.GetComponent<NavMeshAgent>().enabled = false;
				agent.SetStartingPosition(agentData.StartingDoor);
				agent.SetTarget(groupSequence.MeetingPoint);
				agent.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(agentData.MaterialPropertyBlock);
				agent.StartSequence(AgentState.WalkingToMeetingPosition);
				
				//Data only
				OverallData.Instance.AddAgentsLeavingBuilding(1);
			}
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
				if(agent.State == AgentState.WalkingToTargetDoor)
				{
					if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 1f)
					{
						agent.State = AgentState.WaitingEnteringDoor;

						if (!doorAgentQueue.ContainsKey(agent.GetTargetDoor()))
						{
							doorAgentQueue.Add(agent.GetTargetDoor(), new Queue<AgentData>());
						}

						doorAgentQueue[agent.GetTargetDoor()].Enqueue(new AgentData(agent));
					}
				}
				else if (agent.State == AgentState.WalkingToMeetingPosition)
				{
					if (!navMeshAgent.pathPending && navMeshAgent.isActiveAndEnabled && navMeshAgent.remainingDistance <= Random.Range(1f, 2.5f))
					{
						var group = SimulationController.Instance.GroupManager.GetActiveGroup(agent);

						if (group.LeaveDoorTogether)
						{
							continue;
						}
						
						navMeshAgent.isStopped = true;
						agent.State = AgentState.WaitingGroupMembers;

						group.MarkAgentArrived();
					}
				}
			}

			ProcessAgentCreateQueue();
		}
	}
}