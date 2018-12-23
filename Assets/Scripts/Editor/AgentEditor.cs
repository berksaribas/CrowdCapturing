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
        
			EditorGUILayout.LabelField("Agent Starting Door: ", agent.GetStartingDoorName());
			EditorGUILayout.LabelField("Agent Target Door: ", agent.GetTargetDoorName());
		}	
	}
}