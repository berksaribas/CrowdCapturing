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

		public void AssembleGroup(Agent agent, Sequence sequence)
		{
			if (activeGroups.ContainsKey(agent.GetAgentId()))
			{
				return;
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
				return;
			}
			
			
			//TODO: Find a target door
			//TODO: Find a meeting position
			//TODO: Create a GroupSequence
			//TODO: Add that GroupSequence to all included agents in the dictionary
		}
	}
}