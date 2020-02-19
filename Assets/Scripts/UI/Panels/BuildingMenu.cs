using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using World;

namespace UI.Panels
{
    public class BuildingMenu : MonoBehaviour
    {
        private Building focusedBuilding;

        public Text IdleText, StaticText, DynamicText;

        private void Awake()
        {
            ResetCanvas();

            UIState.Building.OnChange += newBuilding =>
            {
                if((focusedBuilding = newBuilding) != null)
                {
                    SetCanvas();
                }
                else
                {
                    ResetCanvas();
                }
            };
        }

        private void OnGUI()
        {
            if (focusedBuilding != null)
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
            
            var text = new StringBuilder();

            text.Append($"{focusedBuilding.name} [{focusedBuilding.DataAlias}]\n\n");
            text.Append($"Weights with other buildings:\n");

            var weights = SimulationController.Instance.BuildingManager.GetBuildingWeights(focusedBuilding.DataAlias);
            var buildingInfos = SimulationController.Instance.BuildingManager.GetBuildingInfos();

            for (var i = 0; i < buildingInfos.Length; i++)
            {
                var buildingInfo = buildingInfos[i];
                var building = buildingInfo.Building;

                if (building != focusedBuilding)
                {
                    text.Append($"{building.name} [{building.DataAlias}]: \t{weights[buildingInfo.Id]}\n");
                }
            }

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            var agentCount = SimulationController.Instance
                .BuildingManager.GetCountOfAgentsInBuilding(
                    focusedBuilding.DataAlias
                );
            
            DynamicText.text = String.Join(
                "\n",
                $"Has {agentCount} agents inside."
            );
        }
    }
}