using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Simulation;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace World
{
    public class BuildingManager : MonoBehaviour
    {
        public TextAsset SequenceCountsMatrix;

        private Building[] buildingIndex;

        public struct BuildingInfo
        {
            public Building Building;
            public int Id;
            public float[] Weights;
        }
        private Dictionary<string, BuildingInfo> buildingInfoMap;
        public BuildingInfo[] GetBuildingInfos() => buildingInfoMap.Values.ToArray();
        public bool HasBuilding(string alias) => buildingInfoMap.ContainsKey(alias);
        public Building GetBuilding(string alias) => buildingInfoMap[alias].Building;
        public float[] GetBuildingWeights(string alias) => buildingInfoMap[alias].Weights;
        
        public void Awake()
        {
            var buildings = FindObjectsOfType<Building>();

            var lines = SequenceCountsMatrix.text
                .Trim()
                .Split('\n');

            var buildingAliases = lines[0]
                .Trim()
                .Split(',')
                .Select(alias => alias.Trim('"'))
                .ToArray();
            
            var sequenceWeights = new Dictionary<string, float[]>(buildingAliases.Length);

            for (var i = 1; i < lines.Length; i++)
            {
                var alias = buildingAliases[i - 1];
                sequenceWeights[alias] = lines[i]
                    .Trim()
                    .Split(',')
                    .Select(weight => float.Parse(weight, CultureInfo.InvariantCulture))
                    .ToArray();
            }

            buildingInfoMap = new Dictionary<string, BuildingInfo>(buildings.Length);
            buildingIndex = new Building[buildingAliases.Length];

            foreach (var building in buildings)
            {
                var orderInData = Array.IndexOf(buildingAliases, building.DataAlias);

                buildingInfoMap.Add(building.DataAlias, new BuildingInfo
                {
                    Building = building,
                    Id = orderInData,
                    Weights = sequenceWeights[building.DataAlias]
                });
                
                // TODO: check if there is any null buildings
                if(building == null) Debug.Log("Building is null");
                
                //    Reorder buildings as in data
                buildingIndex[orderInData] = building;
            }

            InitializeColors();
        }

        //    TODO: be sure that all buildings are not null and remove the where
        public Building[] GetBuildings() => buildingIndex.Where(b => b != null).ToArray();
        public Building GetBuilding(int buildingId) => buildingIndex[buildingId];

        private Door GetDoorByTargetBuilding(Building startingBuilding, Building targetBuilding)
        //    TODO: Move this to GetDoorsFor
        {
            var targetBuildingPosition = targetBuilding.AveragePosition;
            var doorList = new List<WeightedItem<Door>>();
            
            foreach (var possibleDoor in startingBuilding.Doors)
            {
                var distance = Vector3.Distance(
                    targetBuildingPosition,
                    possibleDoor.transform.position
                );
                doorList.Add(new WeightedItem<Door>(possibleDoor, 1f / distance));
            }

            return WeightedItem<Door>.PickWeightedRandom(doorList);
        }

        private Door GetTargetDoorByTargetBuilding(Door selectedDoor, Building targetBuilding)
        //    TODO: Move this to GetDoorsFor
        {
            var doorList = new List<WeightedItem<Door>>();

            foreach (var possibleDoor in targetBuilding.Doors)
            {
                var distance = Vector3.Distance(selectedDoor.transform.position, possibleDoor.transform.position);
                doorList.Add(new WeightedItem<Door>(possibleDoor, Mathf.Pow(1 / distance, 10)));
            }

            return WeightedItem<Door>.PickWeightedRandom(doorList);
        }

        public (Door, Door) FindDoorsFor(Building startingBuilding, Building targetBuilding)
        {
            var startDoor = GetDoorByTargetBuilding(startingBuilding, targetBuilding);
            var targetDoor = GetTargetDoorByTargetBuilding(startDoor, targetBuilding);

            return (startDoor, targetDoor);
        }
        
        // Helpers for coloring agents with respect to their sequence
        private readonly Dictionary<Building, Dictionary<Building, Color>> sequenceColors =
            new Dictionary<Building, Dictionary<Building, Color>>();
        private void InitializeColors()
        {
            foreach (var buildingInfo in buildingInfoMap.Values)
            {
                var colorMap = new Dictionary<Building, Color>(buildingInfoMap.Count);

                foreach (var buildingInfo2 in buildingInfoMap.Values)
                    colorMap[buildingInfo2.Building] = Color.HSVToRGB(
                        Random.Range(0.0f, 1.0f),
                        Random.Range(0.0f, 1.0f),
                        1.0f
                    );

                sequenceColors[buildingInfo.Building] = colorMap;
            }
        }
        public Color GetColor(Sequence s)
        {
            return sequenceColors[s.StartingBuilding][s.TargetBuilding];
        }
    }
}