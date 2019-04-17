using System.Collections.Generic;
using UnityEngine;

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
			return activeGroups.ContainsKey(agent.GetAgentId());
		}

		public GroupSequence GetActiveGroup(Agent agent)
		{
			if (activeGroups.ContainsKey(agent.GetAgentId()))
			{
				return activeGroups[agent.GetAgentId()];
			}

			return null;
		}

		public GroupSequence CreateGroup(Agent agent, Sequence sequence)
		{
			//TODO: Find a target door
			//TODO: Find a meeting position
			//TODO: Create a GroupSequence
			//TODO: Add that GroupSequence to all included agents in the dictionary

			return null;
		}
	}
}