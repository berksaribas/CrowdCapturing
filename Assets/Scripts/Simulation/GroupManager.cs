using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using Data;
using DefaultNamespace;
using UnityEngine;
using World;

namespace Simulation
{
	public class GroupManager : MonoBehaviour
	{
		public float MeetAtDoorThreshold = 80f;
		public int MaxWaitTimeForGroupMembers = 600;
		private Dictionary<int, GroupSequence> activeGroups;

		private HashSet<int> tempSet = new HashSet<int>();
		private List<Agent> sameBuildingStarters = new List<Agent>(5);
		
		private void Awake()
		{
			activeGroups = new Dictionary<int, GroupSequence>();
		}

		public bool CanCreateAGroup(Agent agent, Sequence sequence)
		{
			if (IsMemberOfAGroup(agent))
			{
				return false;
			}

			int includedAgents = 0;
			foreach (var sequenceGroupingAgent in sequence.GroupingAgents)
			{
				if (IsAgentAvailableForGrouping(agent, sequence, sequenceGroupingAgent))
				{
					includedAgents++;
				}
			}

			if (includedAgents == 0)
			{
				return false;
			}

			return true;
		}

		public bool IsMemberOfAGroup(Agent agent)
		{
			return activeGroups.ContainsKey(agent.GetAgentId()) && activeGroups[agent.GetAgentId()] != null;
		}

		public GroupSequence GetActiveGroup(Agent agent)
		{
			if (activeGroups.ContainsKey(agent.GetAgentId()))
			{
				return activeGroups[agent.GetAgentId()];
			}

			return null;
		}

		public void CreateGroup(Agent agent, Sequence sequence, Door startingDoor)
		{
			Door targetDoor = SimulationController.Instance
				.BuildingManager.GetFinishingDoorByTargetBuilding(
					startingDoor,
					sequence.TargetBuildingId
				);

			List<Agent> availableAgents = new List<Agent>();
			foreach (var sequenceGroupingAgent in sequence.GroupingAgents)
			{
				if (IsAgentAvailableForGrouping(agent, sequence, sequenceGroupingAgent))
				{
					availableAgents.Add(SimulationController.Instance.AgentManager.GetAgentById(sequenceGroupingAgent));
				}
			}
			availableAgents.Add(agent);

			tempSet.Clear();
			sameBuildingStarters.Clear();
			var meetingPosition = CalculateMeetingPosition(startingDoor, targetDoor, availableAgents);
			var groupSequence = new GroupSequence(meetingPosition, targetDoor.transform.position, targetDoor);

			var subgroupList = new List<int>();
			
			//Find agents that will leave the building at the same time, create a subgroup for them
			for (int i = 0; i < availableAgents.Count; i++)
			{
				for (int j = i + 1; j < availableAgents.Count; j++)
				{
					if (tempSet.Contains(i) || tempSet.Contains(j))
					{
						continue;
					}
					
					if (availableAgents[i].GetNextSequence().StartingBuildingId == availableAgents[j].GetNextSequence().StartingBuildingId)
					{
						tempSet.Add(j);
						if(!sameBuildingStarters.Contains(availableAgents[i]))
						{
							sameBuildingStarters.Add(availableAgents[i]);
						}
						
						if(!sameBuildingStarters.Contains(availableAgents[j]))
						{
							sameBuildingStarters.Add(availableAgents[j]);
						}
					}
				}
				if(sameBuildingStarters.Count > 1)
				{
					subgroupList.Add(sameBuildingStarters.Count);
					Debug.Log("Same building group!");
					tempSet.Add(i);
					var leaveTogetherGroup = new GroupSequence();
					leaveTogetherGroup.agentCount = sameBuildingStarters.Count;
					leaveTogetherGroup.SetParentGroupSequence(groupSequence);
					foreach (var sameBuildingStarter in sameBuildingStarters)
					{
						Debug.Log($"Same building agent ID: {sameBuildingStarter.GetAgentId()}", sameBuildingStarter);
						leaveTogetherGroup.AddAgent(sameBuildingStarter);
						activeGroups[sameBuildingStarter.GetAgentId()] = leaveTogetherGroup;
					}
				}
				sameBuildingStarters.Clear();
			}

			//Create the main group
			for (var index = 0; index < availableAgents.Count; index++)
			{
				var availableAgent = availableAgents[index];

				if(!tempSet.Contains(index))
				{
					activeGroups[availableAgent.GetAgentId()] = groupSequence;
					subgroupList.Add(1);
				}
				groupSequence.AddAgent(availableAgent);

				groupSequence.debugText += $" - {availableAgent.GetAgentId()} - ";
			}
			groupSequence.agentCount = availableAgents.Count;
			
			OverallData.Instance.AddParentGroup(groupSequence.agentCount);
			OverallData.Instance.AddSubgroupSize(groupSequence.agentCount, subgroupList);
			Debug.Log("A group is created!");
		}

		public void RemoveFromGroup(Agent agent)
		{
			activeGroups[agent.GetAgentId()].RemoveAgent(agent);
			activeGroups[agent.GetAgentId()] = null;
			activeGroups.Remove(agent.GetAgentId());
		}
		
		public void AddToGroup(Agent agent, GroupSequence groupSequence)
		{
			activeGroups[agent.GetAgentId()] = groupSequence;
		}

		private Vector3 CalculateMeetingPosition(Door startingDoor, Door targetDoor, List<Agent> availableAgents)
		{
			var totalPosition = Vector3.zero;
			totalPosition += startingDoor.transform.position;
			totalPosition += targetDoor.transform.position;
			
			foreach (var availableAgent in availableAgents)
			{
				totalPosition += SimulationController.Instance.
					BuildingManager.GetBuilding(
						availableAgent.GetNextSequence().StartingBuildingId
					).AveragePosition;
			}

			var meetingPosition = totalPosition / (2 + availableAgents.Count);

			Debug.Log("For filtering: " + Vector3.Distance(meetingPosition, targetDoor.transform.position));
			if (Vector3.Distance(meetingPosition, targetDoor.transform.position) < MeetAtDoorThreshold)
			{
				meetingPosition = targetDoor.transform.position;
			}

			return meetingPosition;
		}

		private bool IsAgentAvailableForGrouping(Agent agent, Sequence sequence, int sequenceGroupingAgent)
		{
			var otherAgent = SimulationController.Instance.AgentManager.GetAgentById(sequenceGroupingAgent);

			return !activeGroups.ContainsKey(sequenceGroupingAgent) &&
				otherAgent != null &&
				otherAgent.GetNextSequence() != null &&
				otherAgent.GetNextSequence().GroupingAgents.Contains(agent.GetAgentId()) &&
				otherAgent.GetNextSequence().StartTime - sequence.StartTime <= MaxWaitTimeForGroupMembers &&
				otherAgent.GetNextSequence().TargetBuildingId == sequence.TargetBuildingId;
		}
	}
}