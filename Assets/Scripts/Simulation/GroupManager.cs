using System.Collections.Generic;
using UnityEngine;
using World;

namespace Simulation
{
	public class GroupManager : MonoBehaviour
	{
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
				if (!activeGroups.ContainsKey(sequenceGroupingAgent))
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
				if (!activeGroups.ContainsKey(sequenceGroupingAgent))
				{
					availableAgents.Add(SimulationController.Instance.CrowdManager.GetAgentById(sequenceGroupingAgent));
				}
			}
			var totalPosition = Vector3.zero;
			totalPosition += startingDoor.transform.position;
			totalPosition += targetDoor.transform.position;
			
			foreach (var availableAgent in availableAgents)
			{
				totalPosition += availableAgent.transform.position;
			}

			var meetingPosition = totalPosition / (2 + availableAgents.Count);
			
			var groupSequence = new GroupSequence(meetingPosition, targetDoor);
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
		}
	}
}