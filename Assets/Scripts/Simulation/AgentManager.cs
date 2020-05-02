using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Util;
using World;

namespace Simulation
{
	public class AgentManager : MonoBehaviour
	{
		public GameObject AgentPrefab;

		private HashSet<Agent> allAgents;
		private HashSet<Agent> activeAgents;
		private Dictionary<Door, Queue<AgentData>> doorAgentQueue;
		private Dictionary<int, Agent> agentIdMap;

		public HashSet<Agent> GetActiveAgents() => activeAgents;

		private void Awake()
		{
			allAgents = new HashSet<Agent>();
			activeAgents = new HashSet<Agent>();
			agentIdMap = new Dictionary<int, Agent>();
			doorAgentQueue = new Dictionary<Door, Queue<AgentData>>();
		}

		public Agent GetAgentById(int agentId)
		{
			return agentIdMap.ContainsKey(agentId) ? agentIdMap[agentId] : null;
		}

		public void InstantiateAgents(Dictionary<int, List<Sequence>> agents)
		{
			var agentCounter = 0;
			foreach (var keyValuePair in agents)
			{
				var id = keyValuePair.Key;
				var sequences = keyValuePair.Value;
				
				if (sequences.Count > 0)
				{
					agentCounter++;
				
					var agentGameObject = Instantiate(AgentPrefab, transform);
					agentGameObject.SetActive(false);

					var agent = agentGameObject.GetComponent<Agent>();
					agent.Id = id;
					agent.InitializeSequences(sequences);
					
					agentIdMap.Add(id, agent);
					allAgents.Add(agent);
				}
			}
			Debug.Log($"Created agents: {agentCounter}");
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
				var door = keyValuePair.Key;
				var allAgentData = keyValuePair.Value;
				
				while (door.TryToPass() && allAgentData.Any())
				{
					var agentData = allAgentData.Dequeue();
					var agent = agentData.Agent;

					switch (agentData.Type)
					{
						case AgentData.DataType.IndividualMove:
							ProcessIndividualMoveAgent(agentData);
							activeAgents.Add(agent);
							break;
						case AgentData.DataType.GroupMoveBeforeMeet:
							ProcessGroupMoveBeforeMeet(agentData);
							activeAgents.Add(agent);
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

			agent.NavMeshAgent.enabled = false;

			var startingDoor = agentData.StartingDoor;
			
			var targetDoor = SimulationController.Instance
				.BuildingManager.GetFinishingDoorByTargetBuilding(
					startingDoor,
					sequence.TargetBuildingId
				);

			agent.SetStartingPosition(startingDoor);
			agent.SetTarget(targetDoor);

			agent.MeshRenderer.SetPropertyBlock(agentData.MaterialPropertyBlock);

			agent.StartSequence(AgentState.WalkingToTargetDoor);
			
			//Data only
			OverallData.Instance.AddAgentsLeavingBuilding(1);
		}
		
		private void ProcessGroupMoveBeforeMeet(AgentData agentData)
		{
			var agent = agentData.Agent;

			if (SimulationController.Instance.GroupManager.IsMemberOfAGroup(agent))
			{
				Debug.Log("Already a member of a group!");
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

			var groupSequence = SimulationController.Instance.GroupManager.GetActiveGroup(agent);

			if (groupSequence.LeaveDoorTogether)
			{
				foreach (var agentInGroup in groupSequence.agents)
				{
					agentInGroup.gameObject.SetActive(true);
					agentInGroup.NavMeshAgent.enabled = false;
					agentInGroup.SetStartingPosition(agentData.StartingDoor);
					agentInGroup.MeshRenderer.SetPropertyBlock(agentData.MaterialPropertyBlock);
					agentInGroup.StartSequence(AgentState.WalkingToMeetingPosition);
					agentInGroup.GetNextSequence().disabled = true;
					groupSequence.MarkAgentArrived();
					if (!activeAgents.Contains(agentInGroup))
					{
						activeAgents.Add(agentInGroup);
					}
				}

				OverallData.Instance.AddAgentsLeavingBuilding(groupSequence.agents.Count);
			}
			else
			{
				agent.gameObject.SetActive(true);
				agent.NavMeshAgent.enabled = false;
				agent.SetStartingPosition(agentData.StartingDoor);
				agent.SetTarget(groupSequence.MeetingPoint);
				agent.MeshRenderer.SetPropertyBlock(agentData.MaterialPropertyBlock);
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
				var navMeshAgent = agent.NavMeshAgent;
				switch (agent.State)
				{
					case AgentState.WalkingToTargetDoor:
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

						break;
					}
					case AgentState.WalkingToMeetingPosition:
					{
						if (!navMeshAgent.pathPending && navMeshAgent.isActiveAndEnabled && navMeshAgent.remainingDistance <= Random.Range(1f, 2.5f))
						{
							var group = SimulationController.Instance.GroupManager.GetActiveGroup(agent);

							if (group.LeaveDoorTogether)
								continue;
						
							navMeshAgent.isStopped = true;
							agent.State = AgentState.WaitingGroupMembers;

							group.MarkAgentArrived();
						}

						break;
					}
				}
			}

			ProcessAgentCreateQueue();
		}
	}
}