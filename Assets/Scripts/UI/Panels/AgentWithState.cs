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
            // ID.text = Agent.Id.ToString();
            State.text = AgentStateToText(Agent);
        }

        private static string AgentStateToText(Agent agent)
        {
            switch (agent.State)
            {
                // default colors are "black, blue, green, orange, purple, red, white, and yellow."
                case AgentState.Idling:
                    //var buildingName = agent.PeekNextSequence().StartingBuilding.name;
                    return $"<color=red>Waiting inside a building</color>";

                case AgentState.Walking:
                    return $"<color=orange>Walking to {agent.CurrentSequence.TargetBuilding.name}</color>";

                // case AgentState.WalkingToMeetingPosition:
                //     return "<color=purple>Walking to meeting</color>";
                //
                // case AgentState.WalkingToTargetDoor:
                //     return $"<color=green>Walking to {agent.TargetDoor.name}</color>";

                default:
                    return agent.State.ToString();
            }
        }
    }
}