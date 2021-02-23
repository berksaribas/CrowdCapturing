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
            ID.text = agent.Id.ToString();
            if (agent.State == AgentState.Idling)
            {
                Exited.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                Exited.transform.parent.gameObject.SetActive(true);
                Exited.text = agent.CurrentSequence.StartingDoor.name;
            }
            State.text = AgentStateToText(agent);
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