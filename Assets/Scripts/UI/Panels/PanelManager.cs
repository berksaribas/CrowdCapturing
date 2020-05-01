using UnityEngine;

namespace UI.Panels
{
    public class PanelManager : MonoBehaviour
    {
        public Panel
            SimulationControlPanel,
            BuildingPanel,
            AgentPanel,
            GroupPanel,
            HeatmapPanel;

        private void Awake()
        {
            UIState.Building.OnChange += building => BuildingPanel.SetState(building != null);
            UIState.Agent.OnChange += agent => AgentPanel.SetState(agent != null);
            UIState.Group.OnChange += group => GroupPanel.SetState(group != null);
        }

        private void Start()
        {
            SimulationControlPanel.Open();
            BuildingPanel.Close();
            AgentPanel.Close();
            GroupPanel.Close();
            HeatmapPanel.Open();
        }
    }
}