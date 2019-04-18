using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

        [CanBeNull] [NonSerialized] public Building FocusedBuilding = null;

        public Text IdleText, StaticText, DynamicText;

        public MeshRenderer BuildingHighlighter;

        public List<GameObject> WeightHighlighters = new List<GameObject>();

        private string[] buildingAliases;
        private readonly Dictionary<string, float[]> sequenceWeights = new Dictionary<string, float[]>();
        private readonly List<BuildingInfo> buildingInfos = new List<BuildingInfo>();

        private void Awake()
        {
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
        }

        void Start()
        {
            foreach (var building in SimulationController.Instance.Buildings)
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

            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Buildings")))
            {
                FocusedBuilding = hit.transform.gameObject.GetComponent<Building>();

                BuildingHighlighter.enabled = true;
                BuildingHighlighter.transform.position = FocusedBuilding.transform.position;
                BuildingHighlighter.transform.rotation = FocusedBuilding.transform.rotation;
                BuildingHighlighter.transform.localScale = FocusedBuilding.GetComponent<BoxCollider>().size;

                WeightHighlighters.ForEach(Destroy);
                WeightHighlighters.Clear();

                var weights = sequenceWeights[FocusedBuilding.DataAlias];

                foreach (var buildingInfo in buildingInfos.Where(info => info.Building.name != FocusedBuilding.name))
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
                BuildingHighlighter.enabled = false;

                ResetCanvas();

                WeightHighlighters.ForEach(Destroy);
                WeightHighlighters.Clear();
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

            text.Append($"{FocusedBuilding.name} [{FocusedBuilding.DataAlias}]\n\n");
            text.Append($"Weights with other buildings:\n");

            var weights = sequenceWeights[FocusedBuilding.DataAlias];

            foreach (var info in buildingInfos.Where(info => info.Building.name != FocusedBuilding.name))
            {
                text.Append($"{info.Building.name} [{info.Building.DataAlias}]: \t{weights[info.OrderInData]}\n");
            }

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            DynamicText.text = String.Join(
                "\n",
                $"Has {FocusedBuilding.AgentCount} agents inside."
            );
        }
    }
}