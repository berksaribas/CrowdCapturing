using System.Collections;
using System.Collections.Generic;
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
        public AgentSelector AgentSelectorObject;
        private Agent focusedAgent;

        public Text IdleText, StaticText, DynamicText;

        private void Awake()
        {
            ResetCanvas();
        }
        
        // Start is called before the first frame update
        void Start()
        {
            AgentSelectorObject.Observe(newFocus =>
            {
                if((focusedAgent = newFocus) != null)
                {
                    SetCanvas();
                }
                else
                {
                    ResetCanvas();
                }
            });
        }

        // Update is called once per frame
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
        }

        private void UpdateCanvas()
        {
            var text = new StringBuilder();

            text.Append($"From: {focusedAgent.GetStartingDoorName()}");
            text.Append($" @ {TimeHelper.ConvertSecondsToString(focusedAgent.GetNextSequence().StartTime)}\n\n");

            if (GroupManager.IsMemberOfAGroup(focusedAgent))
            {
                switch (focusedAgent.State)
                {
                    case AgentState.WalkingToMeetingPosition:
                        text.Append("Walking to the meeting point.\n");
                        break;

                    case AgentState.WaitingGroupMembers:
                        text.Append("Waiting for other group members.\n");
                        break;

                    case AgentState.WalkingToTargetDoor:
                        text.Append($"Walking to the door '{focusedAgent.GetTargetDoorName()}' with a group.\n");
                        break;

                    default:
                        text.Append($"MISSING STATE MESSAGE FOR '{focusedAgent.State}'\n");
                        break;
                }
            }
            else
            {
                text.Append($"Walking to the door '{focusedAgent.GetTargetDoorName()}'.\n");
            }

            DynamicText.text = text.ToString();
        }
    }
}