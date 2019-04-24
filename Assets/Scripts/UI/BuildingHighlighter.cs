using System.Collections.Generic;
using System.Linq;
using Control;
using UnityEngine;
using World;

namespace UI
{
    public class BuildingHighlighter : MonoBehaviour
    {
        public BuildingSelector BuildingSelector;
        private Building focusedBuilding;

        public MeshRenderer HighlighterObject;

        private readonly List<GameObject> highlighters = new List<GameObject>();

        void Start()
        {
            BuildingSelector.Observe(newFocus =>
            {
                ClearHighlighters();
                
                if ((focusedBuilding = newFocus) != null)
                {
                    HighlighterObject.enabled = true;
                    HighlighterObject.transform.position = focusedBuilding.transform.position;
                    HighlighterObject.transform.rotation = focusedBuilding.transform.rotation;
                    HighlighterObject.transform.localScale = focusedBuilding.GetComponent<BoxCollider>().size;

                    var weights = SimulationController.Instance.BuildingInfoMap[focusedBuilding.DataAlias].Weights;
                    var otherBuildingInfos = SimulationController.Instance.BuildingInfoMap.Values.Where(
                        info => info.Building.name != focusedBuilding.name
                    );

                    foreach (var buildingInfo in otherBuildingInfos)
                    {
                        highlighters.Add(
                            HighlightLine.CreateLine(
                                focusedBuilding.transform.position,
                                buildingInfo.Building.transform.position,
                                transform,
                                weights[buildingInfo.OrderInData] * 10f
                            )
                        );
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