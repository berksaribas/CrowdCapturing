using System.Collections.Generic;
using System.Linq;
using Control;
using UnityEngine;
using World;

namespace UI
{
    public class BuildingHighlighter : MonoBehaviour
    {
        public BuildingSelector BuildingSelectorObject;
        private Building focusedBuilding;

        public MeshRenderer HighlighterObject;

        private readonly List<GameObject> highlighters = new List<GameObject>();

        void Start()
        {
            BuildingSelectorObject.Observe(newFocus =>
            {
                if ((focusedBuilding = newFocus) != null)
                {
                    HighlighterObject.enabled = true;
                    HighlighterObject.transform.position = focusedBuilding.transform.position;
                    HighlighterObject.transform.rotation = focusedBuilding.transform.rotation;
                    HighlighterObject.transform.localScale = focusedBuilding.GetComponent<BoxCollider>().size;

                    highlighters.ForEach(Destroy);
                    highlighters.Clear();

                    var weights = SimulationController.Instance.BuildingInfoMap[focusedBuilding.DataAlias].Weights;
                    var otherBuildingInfos = SimulationController.Instance.BuildingInfoMap.Values.Where(
                        info => info.Building.name != focusedBuilding.name
                    );

                    foreach (var buildingInfo in otherBuildingInfos)
                    {
                        highlighters.Add(
                            HighlightLine.CreateNew(
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

                    highlighters.ForEach(Destroy);
                    highlighters.Clear();
                }
            });
        }
    }
}