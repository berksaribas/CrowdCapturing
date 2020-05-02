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
		private HashSet<Agent> sameBuildingStarters = new HashSet<Agent>();
		
		private void Awake()
		{
			activeGroups = new Dictionary<int, GroupSequence>();
		}

		public bool CanCreateAGroup(Agent agent, Sequence sequence)
		{
			if (IsMemberOfAGroup(agent))
				return false;

			foreach (var sequenceGroupingAgent in sequence.GroupingAgents)
				if (IsAgentAvailableForGrouping(agent, sequence, sequenceGroupingAgent))
					return true;
			
			return false;
		}

		public bool IsMemberOfAGroup(Agent agent)
		{
			return activeGroups.ContainsKey(agent.Id) && activeGroups[agent.Id] != null;
		}

		public GroupSequence GetActiveGroup(Agent agent)
		{
			if (activeGroups.ContainsKey(agent.Id))
				return activeGroups[agent.Id];

			return null;
		}

		public void CreateGroup(Agent agent, Sequence sequence, Door startingDoor)
		{
			var targetDoor = SimulationController.Instance
				.BuildingManager.GetFinishingDoorByTargetBuilding(
					startingDoor,
					sequence.TargetBuildingId
				);
			var availableAgents = new List<Agent> {agent};
			
			foreach (var sequenceGroupingAgent in sequence.GroupingAgents)
				if (IsAgentAvailableForGrouping(agent, sequence, sequenceGroupingAgent))
					availableAgents.Add(SimulationController.Instance.AgentManager.GetAgentById(sequenceGroupingAgent));

			tempSet.Clear();
			sameBuildingStarters.Clear();
			
			var meetingPosition = CalculateMeetingPosition(startingDoor, targetDoor, availableAgents);
			var groupSequence = new GroupSequence(meetingPosition, targetDoor.transform.position, targetDoor);

			var subgroupList = new List<int>();
			
			//Find agents that will leave the building at the same time, create a subgroup for them
			for (var i = 0; i < availableAgents.Count; i++)
			{
				for (var j = i + 1; j < availableAgents.Count; j++)
				{
					if (tempSet.Contains(i) || tempSet.Contains(j))
						continue;
					
					if (availableAgents[i].GetNextSequence().StartingBuildingId == availableAgents[j].GetNextSequence().StartingBuildingId)
					{
						tempSet.Add(j);
						sameBuildingStarters.Add(availableAgents[i]);
						sameBuildingStarters.Add(availableAgents[j]);
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
						Debug.Log($"Same building agent ID: {sameBuildingStarter.Id.ToString()}", sameBuildingStarter);
						leaveTogetherGroup.AddAgent(sameBuildingStarter);
						activeGroups[sameBuildingStarter.Id] = leaveTogetherGroup;
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
					activeGroups[availableAgent.Id] = groupSequence;
					subgroupList.Add(1);
				}
				groupSequence.AddAgent(availableAgent);

				groupSequence.debugText += $" - {availableAgent.Id.ToString()} - ";
			}
			groupSequence.agentCount = availableAgents.Count;
			
			OverallData.Instance.AddParentGroup(groupSequence.agentCount);
			OverallData.Instance.AddSubgroupSize(groupSequence.agentCount, subgroupList);
			Debug.Log("A group is created!");
		}

		public void RemoveFromGroup(Agent agent)
		{
			activeGroups[agent.Id].RemoveAgent(agent);
			activeGroups[agent.Id] = null;
			activeGroups.Remove(agent.Id);
		}
		
		public void AddToGroup(Agent agent, GroupSequence groupSequence)
		{
			activeGroups[agent.Id] = groupSequence;
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
				otherAgent.GetNextSequence().GroupingAgents.Contains(agent.Id) &&
				otherAgent.GetNextSequence().StartTime - sequence.StartTime <= MaxWaitTimeForGroupMembers &&
				otherAgent.GetNextSequence().TargetBuildingId == sequence.TargetBuildingId;
		}
	}
}