using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public GroupManager GroupManager;
        
        public AgentSelector AgentSelector;
        private Agent focusedAgent;
        
        public GroupSelector GroupSelector;
        private GroupSequence focusedGroup;
        
        public Text IdleText, StaticText, DynamicText;

        private void Awake()
        {
            ResetCanvas();
        }

        void Start()
        {
            AgentSelector.Observe(newFocus =>
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
            
            GroupSelector.Observe(newFocus =>
            {
                if ((focusedGroup = newFocus) != null && focusedAgent != null)
                {
                    SetCanvas();
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
            text.AppendLine();

            if (focusedGroup != null)
            {
                text.AppendLine($"Agent has a group with {focusedGroup.agents.Count} members.");
            }
            else
            {
                text.AppendLine("Agent is alone.");
            }

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            var text = new StringBuilder();

            if (focusedGroup != null)
            {
                switch (focusedAgent.State)
                {
                    case AgentState.WalkingToMeetingPosition:
                        text.AppendLine("Walking to the meeting point.");
                        AdGroupStatus(text, true);
                        break;

                    case AgentState.WaitingGroupMembers:
                        text.AppendLine("Waiting for other group members.");
                        AdGroupStatus(text, true);
                        break;

                    case AgentState.WalkingToTargetDoor:
                        text.AppendLine($"Walking to the door '{focusedAgent.GetTargetDoorName()}' with a group.");
                        AdGroupStatus(text, false);
                        break;

                    case AgentState.WaitingLeavingDoor:
                    case AgentState.WaitingEnteringDoor:
                        text.AppendLine("Passing through a door.");
                        AdGroupStatus(text, false);
                        break;
                    
                    case AgentState.Idling:
                        text.AppendLine("Agent reached the target.");
                        AdGroupStatus(text, true);
                        break;
                }
            }
            else
            {
                text.AppendLine($"Walking to the door '{focusedAgent.GetTargetDoorName()}'.");
            }

            DynamicText.text = text.ToString();
        }

        private void AdGroupStatus(StringBuilder text, bool includeMemberStatus)
        {
            text.AppendLine();
            text.AppendLine("Group Members:");

            foreach (var agent in focusedGroup.agents.Where(a => a != focusedAgent))
            {
                text.Append($"Agent {agent.GetAgentId()}");

                if (includeMemberStatus)
                {
                    text.Append(": ");
                    text.AppendLine(
                        agent.State == AgentState.WaitingGroupMembers ?
                            "<color=lime>Waiting</color>"
                            :
                            "<color=red>Walking</color>"
                        );
                }
                else
                {
                    text.AppendLine();
                }
            }
        }
    }
}