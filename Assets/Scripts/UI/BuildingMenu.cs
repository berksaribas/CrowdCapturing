using System;
using System.Linq;
using System.Text;
using Control;
using Simulation;
using UnityEngine;
using UnityEngine.UI;
using World;

namespace UI
{
    public class BuildingMenu : MonoBehaviour
    {
        public BuildingSelector BuildingSelectorObject;
        private Building focusedBuilding;

        public Text IdleText, StaticText, DynamicText;

        private void Awake()
        {
            ResetCanvas();
        }
        
        void Start()
        {
            BuildingSelectorObject.Observe(newFocus =>
            {
                if((focusedBuilding = newFocus) != null)
                {
                    SetCanvas();
                }
                else
                {
                    ResetCanvas();
                }
            });
        }

        private void Update()
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

            var weights = SimulationController.Instance.BuildingInfoMap[focusedBuilding.DataAlias].Weights;
            var otherBuildingInfos = SimulationController.Instance.BuildingInfoMap.Values.Where(
                info => info.Building.name != focusedBuilding.name
            );

            foreach (var info in otherBuildingInfos)
            {
                text.Append($"{info.Building.name} [{info.Building.DataAlias}]: \t{weights[info.OrderInData]}\n");
            }

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            DynamicText.text = String.Join(
                "\n",
                $"Has {focusedBuilding.AgentCount} agents inside."
            );
        }
    }
}