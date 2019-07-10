using DefaultNamespace;
using Simulation;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(OverallData))]
	public class OverallDataEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var data = (OverallData) target;
			
			EditorGUILayout.LabelField($"Active agents: {GameObject.FindObjectsOfType<Agent>().Length}");
			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
			EditorGUILayout.LabelField("People leaving building together:");
			foreach (var keyValuePair in data.PeopleLeavingBuilding)
			{
				EditorGUILayout.LabelField($"Group Size: {keyValuePair.Key}, count: {keyValuePair.Value}");
			}
			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
			
			EditorGUILayout.LabelField("Constructed Parent Groups:");
			foreach (var keyValuePair in data.ParentGroups)
			{
				EditorGUILayout.LabelField($"Group Size: {keyValuePair.Key}, count: {keyValuePair.Value}");
			}
			
			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
		}
	}
}