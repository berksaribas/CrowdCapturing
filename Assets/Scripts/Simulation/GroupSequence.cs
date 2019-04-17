using System.Collections.Generic;
using UnityEngine;
using World;

namespace Simulation
{
	public class GroupSequence
	{
		public Vector3 MeetingPoint;
		public Door TargetDoor;
		private List<Agent> agents;
		private int arrivedAgents;

		public GroupSequence(Vector3 meetingPoint, Door targetDoor)
		{
			MeetingPoint = meetingPoint;
			TargetDoor = targetDoor;
			
			agents = new List<Agent>();
		}

		public void AddAgent(Agent agent)
		{
			agents.Add(agent);
		}
		
		public void RemoveAgent(Agent agent)
		{
			agents.Remove(agent);
		}

		public void MarkAgentArrived()
		{
			arrivedAgents ++;

			if (arrivedAgents == agents.Count)
			{
				MakeAgentsMoveTargetBuilding();
			}
		}

		private void MakeAgentsMoveTargetBuilding()
		{
			foreach (var agent in agents)
			{
				agent.SetTarget(TargetDoor);
				agent.State = AgentState.WalkingToTargetDoor;
			}
		}
	}
}