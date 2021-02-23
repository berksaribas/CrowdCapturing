using System.Collections.Generic;
using Simulation;
using UnityEngine;
using World;

namespace UI.InGame
{
    public class BuildingHighlighter : MonoBehaviour
    {
        private Building focusedBuilding;

        public MeshRenderer HighlighterObject;

        private readonly List<GameObject> highlighters = new List<GameObject>();

        void Start()
        {
            UIState.Building.OnChange += newBuilding =>
            {
                ClearHighlighters();
                
                if ((focusedBuilding = newBuilding) != null)
                {
                    HighlighterObject.enabled = true;
                    HighlighterObject.transform.position = focusedBuilding.transform.position;
                    HighlighterObject.transform.rotation = focusedBuilding.transform.rotation;
                    HighlighterObject.transform.localScale = focusedBuilding.GetComponent<BoxCollider>().size;

                    var weights = SimulationController.Instance.BuildingManager.GetBuildingWeights(focusedBuilding.DataAlias);
                    var buildingInfos = SimulationController.Instance.BuildingManager.GetBuildingInfos();

                    foreach (var buildingInfo in buildingInfos)
                        if (buildingInfo.Building != focusedBuilding)
                        {
                            highlighters.Add(
                                HighlightLine.CreateLine(
                                    focusedBuilding.transform.position,
                                    buildingInfo.Building.transform.position,
                                    transform,
                                    weights[buildingInfo.Id] * 10f
                                )
                            );
                        }
                }
                else
                {
                    HighlighterObject.enabled = false;
                }
            };
        }

        private void ClearHighlighters()
        {
            highlighters.ForEach(Destroy);
            highlighters.Clear();
        }
    }
}