using System.Collections.Generic;
using UnityEngine;
using World;

namespace Simulation
{
	public class GroupManager : MonoBehaviour
	{
		public float MeetAtDoorThreshold = 80f;
		public int MaxWaitTimeForGroupMembers = 600;
		private Dictionary<int, GroupSequence> activeGroups;

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
				var otherAgent = SimulationController.Instance.CrowdManager.GetAgentById(sequenceGroupingAgent);
				if (!activeGroups.ContainsKey(sequenceGroupingAgent) &&
				    otherAgent.GetNextSequence() != null &&
				    otherAgent.GetNextSequence().GroupingAgents.Contains(agent.GetAgentId()) &&
				    otherAgent.GetNextSequence().StartTime - sequence.StartTime <= MaxWaitTimeForGroupMembers &&
				    otherAgent.GetNextSequence().TargetBuilding == sequence.TargetBuilding)
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

		public GroupSequence CreateGroup(Agent agent, Sequence sequence, Door startingDoor)
		{
			Door targetDoor = sequence.StartingBuilding.GetFinishingDoorByTargetBuilding(startingDoor,
				sequence.TargetBuilding);

			List<Agent> availableAgents = new List<Agent>();
			foreach (var sequenceGroupingAgent in sequence.GroupingAgents)
			{
				var otherAgent = SimulationController.Instance.CrowdManager.GetAgentById(sequenceGroupingAgent);

				if (!activeGroups.ContainsKey(sequenceGroupingAgent) &&
				    otherAgent.GetNextSequence() != null &&
				    otherAgent.GetNextSequence().GroupingAgents.Contains(agent.GetAgentId()) &&
				    otherAgent.GetNextSequence().StartTime - sequence.StartTime <= MaxWaitTimeForGroupMembers &&
				    otherAgent.GetNextSequence().TargetBuilding == sequence.TargetBuilding)
				{
					availableAgents.Add(SimulationController.Instance.CrowdManager.GetAgentById(sequenceGroupingAgent));
				}
			}

			var meetingPosition = CalculateMeetingPosition(startingDoor, targetDoor, availableAgents);
			
			var groupSequence = new GroupSequence(meetingPosition, targetDoor.transform.position, targetDoor);
			groupSequence.AddAgent(agent);
			activeGroups[agent.GetAgentId()] = groupSequence;
			
			foreach (var availableAgent in availableAgents)
			{
				groupSequence.AddAgent(availableAgent);
				activeGroups[availableAgent.GetAgentId()] = groupSequence;
			}

			Debug.Log("A group is created!");
			
			return groupSequence;
		}

		public void RemoveFromGroup(Agent agent)
		{
			activeGroups[agent.GetAgentId()].RemoveAgent(agent);
			activeGroups[agent.GetAgentId()] = null;
			activeGroups.Remove(agent.GetAgentId());
		}

		private Vector3 CalculateMeetingPosition(Door startingDoor, Door targetDoor, List<Agent> availableAgents)
		{
			var totalPosition = Vector3.zero;
			totalPosition += startingDoor.transform.position;
			totalPosition += targetDoor.transform.position;
			
			foreach (var availableAgent in availableAgents)
			{
				totalPosition += availableAgent.GetNextSequence().StartingBuilding.AveragePosition;
			}

			var meetingPosition = totalPosition / (2 + availableAgents.Count);

			Debug.Log("For filtering: " + Vector3.Distance(meetingPosition, targetDoor.transform.position));
			if (Vector3.Distance(meetingPosition, targetDoor.transform.position) < MeetAtDoorThreshold)
			{
				meetingPosition = targetDoor.transform.position;
			}

			return meetingPosition;
		}
	}
}