using Simulation;
using UnityEditor;

namespace EditorScripts
{
	[CustomEditor(typeof(Agent))]
	public class AgentEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var agent = (Agent) target;
			var groupManager = SimulationController.Instance.GroupManager;
			EditorGUILayout.LabelField("Agent State: ", $"{agent.State}");

			if (groupManager.IsMemberOfAGroup(agent))
			{
				var group = groupManager.GetActiveGroup(agent);
				EditorGUILayout.LabelField("Arrived Members: ", $"{group.ArrivedAgents}");
				EditorGUILayout.LabelField("Total Members: ", $"{group.agents.Count}");
				foreach (var groupAgent in group.agents)
				{
					EditorGUILayout.ObjectField(groupAgent, typeof(Agent));
				}
			}
			else
			{
				EditorGUILayout.LabelField("Agent Starting Door: ", agent.GetStartingDoorName());
				EditorGUILayout.LabelField("Agent Target Door: ", agent.GetTargetDoorName());
			}
			EditorGUILayout.LabelField("Agent ID: ", $"{agent.GetAgentId()}");
			if(agent.GetNextSequence() != null)
			{
				EditorGUILayout.LabelField("Next sequence start time: ", $"{agent.GetNextSequence().StartTime} / ${SimulationController.Instance.SimulationManager.WorldTimeSeconds}");
			}
		}	
	}
}