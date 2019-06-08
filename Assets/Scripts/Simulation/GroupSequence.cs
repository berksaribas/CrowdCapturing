using System.Collections.Generic;
using UnityEngine;
using World;

namespace Simulation
{
	public class GroupSequence
	{
		public Door TargetDoor;
		public Vector3 MeetingPoint;
		public Vector3 TargetPoint;
		public int ArrivedAgents => arrivedAgents;

		public readonly List<Agent> agents;
		private int arrivedAgents;
		private GroupSequence parentGroupSequence;

		public GroupSequence(Vector3 meetingPoint, Vector3 targetPoint, Door targetDoor)
		{
			MeetingPoint = meetingPoint;
			TargetPoint = targetPoint;
			TargetDoor = targetDoor;
			agents = new List<Agent>();
		}

		public void SetParentGroupSequence(GroupSequence groupSequence)
		{
			parentGroupSequence = groupSequence;
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

			Debug.Log("A group member has arrived to the meeting point!");
			
			if (arrivedAgents == agents.Count)
			{
				MakeAgentsMoveToTarget();
			}
		}

		private void MakeAgentsMoveToTarget()
		{
			//TODO: If parentGroupSequence is null, run the code on bottom
			//TODO: otherwise call GroupManager.RemoveFromGroup
			//TODO: call GroupManager.ActivateGroup
			//TODO: set state to WalkingToMeetingPosition
			Debug.Log("Group members are going to the target building together!");

			for (var index = 0; index < agents.Count; index++)
			{
				var agent = agents[index];
//				agent.SetTarget(TargetPoint);
				agent.SetTarget(TargetDoor);
				agent.State = AgentState.WalkingToTargetDoor;
				agent.SetSpeed(agents[0].GetSpeed());

				var groupAgentTimer = agent.gameObject.GetComponent<GroupAgentTimer>();
				if (groupAgentTimer != null)
				{
					groupAgentTimer.EndWaitingTime = SimulationController.Instance.SimulationManager.WorldTimeSeconds;
				}
			}
		}
	}
}