using System;
using Simulation;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class AgentWithState : MonoBehaviour
    {
        public AgentLink AgentLink;
        public TextMeshProUGUI /*ID,*/ State;

        private Agent agent;
        public Agent Agent
        {
            get => agent;
            set
            {
                agent = value;

                gameObject.SetActive(agent != null);
                
                AgentLink.Agent = agent;
            }
        }

        private void OnGUI()
        {
            // ID.text = Agent.GetAgentId().ToString();
            State.text = AgentStateToText(Agent);
        }

        private static string AgentStateToText(Agent agent)
        {
            switch (agent.State)
            {
                // "black, blue, green, orange, purple, red, white, and yellow."
                case AgentState.Idling:
                    var buildingName = SimulationController.Instance
                        .BuildingManager.GetBuilding(
                            agent.GetNextSequence().StartingBuildingId
                        ).name;
                    return $"<color=red>Waiting inside {buildingName}</color>";

                case AgentState.WaitingGroupMembers:
                    return "<color=orange>Waiting</color>";

                case AgentState.WalkingToMeetingPosition:
                    return "<color=purple>Walking to meeting</color>";

                case AgentState.WalkingToTargetDoor:
                    return $"<color=green>Walking to {agent.GetTargetDoorName()}</color>";

                case AgentState.WaitingLeavingDoor:
                case AgentState.WaitingEnteringDoor:
                    return "Passing through a door.";

                default:
                    return agent.State.ToString();
            }
        }
    }
}