using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class SimulationStatistics : MonoBehaviour
    {
        public TextMeshProUGUI AgentCount;

        private void OnGUI()
        {
            var count = SimulationController.Instance.AgentManager.GetAgents().Count.ToString();
            AgentCount.text = $"<mspace=0.6em>{count}</mspace>";
        }
    }
}