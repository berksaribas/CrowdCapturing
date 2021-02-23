using Simulation;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Agent))]
    public class AgentEditor : UnityEditor.Editor
    {
        //    TODO
        // public override void OnInspectorGUI()
        // {
        //     if (!Application.isPlaying)
        //     {
        //         DrawDefaultInspector();
        //         return;
        //     }
        //
        //     var agent = (Agent) target;
        //     var groupManager = SimulationController.Instance.GroupManager;
        //     EditorGUILayout.LabelField("Agent State: ", $"{agent.State}");
        //
        //     if (groupManager.IsMemberOfAGroup(agent))
        //     {
        //         var group = groupManager.GetActiveGroup(agent);
        //         EditorGUILayout.LabelField("Arrived Members: ", group.ArrivedAgents.ToString());
        //         EditorGUILayout.LabelField("Total Members: ", group.agents.Count.ToString());
        //         foreach (var groupAgent in group.agents)
        //         {
        //             EditorGUILayout.ObjectField(groupAgent, typeof(Agent), true);
        //         }
        //     }
        //     else
        //     {
        //         EditorGUILayout.LabelField("Agent Starting Door: ", agent.StartingDoor.name);
        //         EditorGUILayout.LabelField("Agent Target Door: ", agent.TargetDoor.name);
        //     }
        //
        //     EditorGUILayout.LabelField("Agent ID: ", agent.Id.ToString());
        //     if (agent.GetNextSequence() != null)
        //     {
        //         EditorGUILayout.LabelField(
        //             "Next sequence start time: ",
        //             $"{agent.GetNextSequence().StartTime.ToString()} / {SimulationController.Instance.TimeManager.TimeInSeconds.ToString()}"
        //         );
        //     }
        // }
    }
}