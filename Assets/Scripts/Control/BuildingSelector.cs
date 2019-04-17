using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using UI;
using UnityEngine;
using UnityEngine.UI;
using World;

namespace Control
{
    public class BuildingSelector : MonoBehaviour
    {
        private struct BuildingInfo
        {
            public Building Building;
            public int OrderInData;
        }

        public Building[] Buildings;

        [CanBeNull] [NonSerialized] public Building FocusedBuilding = null;

        public GameObject CanvasObject;
        private RectTransform canvas;

        public GameObject HighlighterObject;
        private MeshRenderer highlightBox;

        public List<GameObject> WeightHighlighters = new List<GameObject>();

        private string[] buildingAliases;
        private readonly Dictionary<string, float[]> sequenceWeights = new Dictionary<string, float[]>();
        private readonly List<BuildingInfo> buildingInfos = new List<BuildingInfo>();

        private void Awake()
        {
            canvas = CanvasObject.GetComponent<RectTransform>();
            highlightBox = HighlighterObject.GetComponent<MeshRenderer>();

            ResetCanvas();

            ReadSequenceCounts();
        }

        private void ReadSequenceCounts()
        {
            var csv = (TextAsset) Resources.Load("sequence_counts_matrix");

            var lines = csv.text.Trim().Split('\n');

            buildingAliases = lines[0].Trim().Split(',')
                .Select(alias => alias.Trim('"'))
                .ToArray();

            for (int i = 1; i < lines.Length; i++)
            {
                float[] weights = lines[i].Trim().Split(',')
                    .Select(weight => float.Parse(weight, CultureInfo.InvariantCulture))
                    .ToArray();
                sequenceWeights[buildingAliases[i - 1]] = weights;
            }

            foreach (var building in Buildings)
            {
                buildingInfos.Add(new BuildingInfo
                {
                    Building = building,
                    OrderInData = Array.IndexOf(buildingAliases, building.DataAlias)
                });
            }
        }

        private void Update()
        {
            if (FocusedBuilding != null)
            {
                UpdateCanvas();
            }

            if (!Input.GetMouseButtonDown(0))
                return;

            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask(new string[] {"Buildings"})))
            {
                FocusedBuilding = hit.transform.gameObject.GetComponent<Building>();

                highlightBox.enabled = true;
                highlightBox.transform.position = FocusedBuilding.transform.position;
                highlightBox.transform.rotation = FocusedBuilding.transform.rotation;
                highlightBox.transform.localScale = FocusedBuilding.GetComponent<BoxCollider>().size;

                WeightHighlighters.ForEach(Destroy);
                WeightHighlighters.Clear();
                
                var weights = sequenceWeights[FocusedBuilding.DataAlias];

                foreach (var buildingInfo in buildingInfos)
                {
                    WeightHighlighters.Add(
                        HighlightLine.CreateNew(
                            FocusedBuilding.transform.position,
                            buildingInfo.Building.transform.position,
                            transform,
                            weights[buildingInfo.OrderInData] * 10f
                        )
                    );
                }

                SetCanvas();
            }
            else
            {
                FocusedBuilding = null;
                highlightBox.enabled = false;

                ResetCanvas();

                WeightHighlighters.ForEach(Destroy);
                WeightHighlighters.Clear();
            }
        }

        private void ResetCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            canvas.GetChild(0).GetComponent<Text>().text = "Select A Building";
        }

        private void SetCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().alignment = TextAnchor.UpperLeft;
        }

        private void UpdateCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().text = String.Join(
                "\n",
                new[]
                {
                    $"{FocusedBuilding.name} [{FocusedBuilding.DataAlias}]",
                    "",
                    $"Has {FocusedBuilding.AgentCount.ToString()} agents inside.",
                }
            );
        }
    }
}