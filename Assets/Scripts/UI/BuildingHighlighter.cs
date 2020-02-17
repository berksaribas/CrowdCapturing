using System.Collections.Generic;
using System.Linq;
using Control;
using UI.State;
using UnityEngine;
using World;

namespace UI
{
    public class BuildingHighlighter : MonoBehaviour
    {
        private Building focusedBuilding;

        public MeshRenderer HighlighterObject;

        private readonly List<GameObject> highlighters = new List<GameObject>();

        void Start()
        {
            FocusedBuilding.Observe(newFocus =>
            {
                ClearHighlighters();
                
                if ((focusedBuilding = newFocus) != null)
                {
                    HighlighterObject.enabled = true;
                    HighlighterObject.transform.position = focusedBuilding.transform.position;
                    HighlighterObject.transform.rotation = focusedBuilding.transform.rotation;
                    HighlighterObject.transform.localScale = focusedBuilding.GetComponent<BoxCollider>().size;

                    var weights = SimulationController.Instance.BuildingManager.GetBuildingWeights(focusedBuilding.DataAlias);
                    var buildingInfos = SimulationController.Instance.BuildingManager.GetBuildingInfos();

                    for (var i = 0; i < buildingInfos.Length; i++)
                    {
                        var buildingInfo = buildingInfos[i];

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
                }
                else
                {
                    HighlighterObject.enabled = false;
                }
            });
        }

        private void ClearHighlighters()
        {
            highlighters.ForEach(Destroy);
            highlighters.Clear();
        }
    }
}