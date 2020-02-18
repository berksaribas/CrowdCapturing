using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class PanelManager : MonoBehaviour
    {
        public Panel
            SimulationControlPanel,
            BuildingPanel,
            AgentPanel,
            HeatmapPanel;

        private void Awake()
        {
            UIState.Building.OnChange += building => BuildingPanel.SetState(building != null);
            UIState.Agent.OnChange += agent => AgentPanel.SetState(agent != null);
        }

        private void Start()
        {
            SimulationControlPanel.Open();
            BuildingPanel.Close();
            AgentPanel.Close();
            HeatmapPanel.Open();
            
            //    TODO: Somehow trigger the layout builder at start
        }
    }
}