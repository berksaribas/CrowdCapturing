using System.Text;
using Control;
using Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class GroupMenu : MonoBehaviour
    {
        private GroupSequence focusedGroup;
        
        public Text IdleText, StaticText, DynamicText;

        private void Awake()
        {
            ResetCanvas();

            UIState.Group.OnChange += newGroup =>
            {
                if ((focusedGroup = newGroup) != null)
                {
                    SetCanvas();
                }
                else
                {
                    ResetCanvas();
                }
            };
        }

        private void OnGUI()
        {
            if (focusedGroup != null)
            {
                UpdateCanvas();
            }
        }

        private void ResetCanvas()
        {
            IdleText.enabled = true;
            StaticText.enabled = false;
            DynamicText.enabled = false;
        }

        private void SetCanvas()
        {
            IdleText.enabled = false;
            StaticText.enabled = true;
            DynamicText.enabled = true;

            var text = new StringBuilder();

            text.AppendLine($"Group with current {focusedGroup.agents.Count} members.");
            text.AppendLine($"Group with a total of {focusedGroup.agentCount} members.");
            text.AppendLine($"Debug text: {focusedGroup.debugText}");
            text.AppendLine();
            text.AppendLine($"Group target is '{focusedGroup.TargetDoor.name}'.");

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            var text = new StringBuilder();

            text.AppendLine();
            text.AppendLine("Group Members:");

            foreach (var agent in focusedGroup.agents)
            {
                text.Append($"Agent {agent.GetAgentId()}: ");

                switch (agent.State)
                {
                    case AgentState.Idling:
                        var building = SimulationController.Instance
                            .BuildingManager.GetBuilding(
                                agent.GetNextSequence().StartingBuildingId
                            );
                        text.AppendLine($"<color=olive>Inside {building.name}</color>");
                        break;
                    
                    case AgentState.WaitingGroupMembers:
                        text.AppendLine($"<color=lime>Waiting</color>");
                        break;
                    
                    case AgentState.WalkingToMeetingPosition:
                        text.AppendLine($"<color=red>Walking to meeting</color>");
                        break;
                        
                    case AgentState.WalkingToTargetDoor:
                        text.AppendLine($"<color=lightblue>Walking to target door</color>");
                        break;
                    
                    default:
                        text.AppendLine(agent.State.ToString());
                        break;
                }
            }
                
            DynamicText.text = text.ToString();
        }
    }
}