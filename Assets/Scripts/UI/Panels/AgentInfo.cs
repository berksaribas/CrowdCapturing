using Simulation;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class AgentInfo : MonoBehaviour
    {
        public TextMeshProUGUI ID, Exited, State;

        private Agent agent;
        public Agent Agent
        {
            get => agent;
            set
            {
                agent = value;
                
                gameObject.SetActive(agent != null);
            }
        }
        
        private void Awake()
        {
            UIState.Agent.OnChange += agent => Agent = agent;
            Agent = UIState.Agent.Get();
        }

        private void OnGUI()
        {
            ID.text = agent.GetAgentId().ToString();
            if (agent.State == AgentState.Idling)
            {
                Exited.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                Exited.transform.parent.gameObject.SetActive(true);
                Exited.text = agent.GetStartingDoorName();
            }
            State.text = AgentStateToText(agent);
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