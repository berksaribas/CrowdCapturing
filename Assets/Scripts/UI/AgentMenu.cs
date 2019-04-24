using System.Text;
using Control;
using Simulation;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class AgentMenu : MonoBehaviour
    {
        public AgentAndGroupSelector AgentAndGroupSelector;
        private Agent focusedAgent;
        
        public Text IdleText, StaticText, DynamicText;

        private void Awake()
        {
            ResetCanvas();
        }

        void Start()
        {
            AgentAndGroupSelector.ObserveAgent(newFocus =>
            {
                if ((focusedAgent = newFocus) != null)
                {
                    SetCanvas();
                }
                else
                {
                    ResetCanvas();
                }
            });
        }

        void Update()
        {
            if (focusedAgent != null)
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

            text.AppendLine($"Agent ID: {focusedAgent.GetAgentId()}");
            text.AppendLine();
            text.Append($"Exited from '{focusedAgent.GetStartingDoorName()}'");
            text.AppendLine($" at {TimeHelper.ConvertSecondsToString(focusedAgent.GetNextSequence().StartTime)}\n");

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            var text = new StringBuilder();
            
            switch (focusedAgent.State)
            {
                case AgentState.WalkingToMeetingPosition:
                    text.AppendLine("Walking to the meeting point.");
                    break;

                case AgentState.WaitingGroupMembers:
                    text.AppendLine("Waiting for other group members.");
                    break;

                case AgentState.WalkingToTargetDoor:
                    text.AppendLine($"Walking to the door '{focusedAgent.GetTargetDoorName()}'.");
                    break;

                case AgentState.WaitingLeavingDoor:
                case AgentState.WaitingEnteringDoor:
                    text.AppendLine("Passing through a door.");
                    break;
                
                case AgentState.Idling:
                    text.AppendLine($"Agent is inside {focusedAgent.GetNextSequence().StartingBuilding.name}.");
                    break;
            }
            
            DynamicText.text = text.ToString();
        }
    }
}