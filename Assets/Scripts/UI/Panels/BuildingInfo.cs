using System;
using TMPro;
using UnityEngine;
using World;

namespace UI.Panels
{
    public class BuildingInfo : MonoBehaviour
    {
        public TextMeshProUGUI Name, ID, AgentCount;

        private Building building;

        public Building Building
        {
            get => building;
            set
            {
                building = value;

                gameObject.SetActive(building != null);
            }
        }

        private void Awake()
        {
            UIState.Building.OnChange += building => Building = building;
            Building = UIState.Building.Get();
        }

        private void OnGUI()
        {
            Name.text = building.name;
            ID.text = building.DataAlias;
            
            // text.Append($"Weights with other buildings:\n");
            // var weights = SimulationController.Instance.BuildingManager.GetBuildingWeights(building.DataAlias);
            // var buildingInfos = SimulationController.Instance.BuildingManager.GetBuildingInfos();
            
            // for (var i = 0; i < buildingInfos.Length; i++)
            // {
            //     var buildingInfo = buildingInfos[i];
            //     var building = buildingInfo.Building;
            //
            //     if (building != focusedBuilding)
            //     {
            //         text.Append($"{building.name} [{building.DataAlias}]: \t{weights[buildingInfo.Id]}\n");
            //     }
            // }
            //
            // StaticText.text = text.ToString();
            
            var agentCount = SimulationController.Instance
                .BuildingManager.GetCountOfAgentsInBuilding(
                    building.DataAlias
                );
            AgentCount.text = $"<mspace=0.6em>{agentCount}</mspace>";
        }
    }
}