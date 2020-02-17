using System.Collections.Generic;
using DefaultNamespace;
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
		public int agentCount = 0;

		public readonly List<Agent> agents;
		public readonly List<int> arrivalTimes;
		private int arrivedAgents;
		private GroupSequence parentGroupSequence;

		public bool LeaveDoorTogether = false;

		public string debugText = "";

		public GroupSequence(Vector3 meetingPoint, Vector3 targetPoint, Door targetDoor)
		{
			MeetingPoint = meetingPoint;
			TargetPoint = targetPoint;
			TargetDoor = targetDoor;
			agents = new List<Agent>();
			arrivalTimes = new List<int>();
		}

		public GroupSequence()
		{
			LeaveDoorTogether = true;
			agents = new List<Agent>();
			arrivalTimes = new List<int>();
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
			if (LeaveDoorTogether)
			{
				Debug.Log("Leave together marked by");
			}
			arrivedAgents ++;

			Debug.Log("A group member has arrived to the meeting point!");
			
			
			if (arrivedAgents == agentCount)
			{
				MakeAgentsMoveToTarget();
			}
			else
			{
				arrivalTimes.Add((int)SimulationController.Instance.SimulationTime.TimeInSeconds);
			}
		}

		private void MakeAgentsMoveToTarget()
		{
			var parentGroupExists = parentGroupSequence != null;
			Debug.Log("Group members are going to the target together!");

			for (var index = 0; index < agents.Count; index++)
			{
				var agent = agents[index];
				agent.SetSpeed(agents[0].GetSpeed());

				if (parentGroupExists)
				{
					Debug.Log("Same building group setting their targets");
					if (LeaveDoorTogether)
					{
						agent.SetTarget(parentGroupSequence.MeetingPoint);
					} else
					{
						agent.SetTarget(TargetPoint);
					}
					agent.State = AgentState.WalkingToMeetingPosition;
					SimulationController.Instance.GroupManager.AddToGroup(agent, parentGroupSequence);
				}
				else
				{
					agent.SetTarget(TargetDoor);
					agent.State = AgentState.WalkingToTargetDoor;
				}
			}

			if (!parentGroupExists)
			{
				foreach (var arrivalTime in arrivalTimes)
				{
					var time = (int) SimulationController.Instance.SimulationTime.TimeInSeconds;
					OverallData.Instance.AddWaitTime(agentCount, time - arrivalTime);
				}
			}
		}
	}
}