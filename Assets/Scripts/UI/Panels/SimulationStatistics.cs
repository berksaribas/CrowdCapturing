using Simulation;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class SimulationStatistics : MonoBehaviour
    {
        public TextMeshProUGUI AgentCount;

        private void OnGUI()
        {
            var count = Agent.AgentsInStates[AgentState.Walking].Count.ToString();
            AgentCount.text = $"<mspace=0.6em>{count}</mspace>";
        }
    }
}