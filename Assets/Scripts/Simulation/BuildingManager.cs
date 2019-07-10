using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Util;
using World;

namespace Simulation
{
    public class BuildingManager : MonoBehaviour
    {
        public GameObject BuildingsParent;
        public TextAsset SequenceCountsMatrix;

        public struct BuildingInfo
        {
            public Building Building;
            public int Id;
            public float[] Weights;
        }

        private Building[] buildingIndex;
        private Dictionary<string, BuildingInfo> buildingInfoMap;
        private List<Agent>[] agentsInBuildings;

        public void Awake()
        {
            var buildings = BuildingsParent.GetComponentsInChildren<Building>();

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
                var aliase = buildingAliases[i - 1];
                sequenceWeights[aliase] = lines[i]
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
                
                //    Reorder buildings as in data
                buildingIndex[orderInData] = building;
            }

            agentsInBuildings = new List<Agent>[buildingAliases.Length];
            for (int i = 0; i < buildingAliases.Length; i++)
            {
                agentsInBuildings[i] = new List<Agent>();
            }
        }

        public bool HasBuilding(string alias)
        {
            return buildingInfoMap.ContainsKey(alias);
        }

        public int GetBuildingId(string alias)
        {
            return buildingInfoMap[alias].Id;
        }

        public float[] GetBuildingWeights(string alias)
        {
            return buildingInfoMap[alias].Weights;
        }

        public Building[] GetBuildings()
        {
            return buildingIndex.Where(b => b!= null).ToArray();
        }

        public Building GetBuilding(int buildingId)
        {
            return buildingIndex[buildingId];
        }

        public void RegisterAgent(int buildingId, Agent agent)
        {
            agentsInBuildings[buildingId].Add(agent);
        }

        public void UnregisterAgent(int buildingId, Agent agent)
        {
            agentsInBuildings[buildingId].Remove(agent);
        }

        public int GetCountOfAgentsInBuilding(string alias)
        {
            return agentsInBuildings[
                GetBuildingId(alias)
            ].Count;
        }
        
        
        public Door GetDoorByTargetBuilding(int startingBuildingId, int targetBuildingId)
        //    TODO: Move this to DoorManager
        {
            List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();
            
            foreach (var possibleDoor in GetBuilding(startingBuildingId).Doors)
            {
                var distance = Vector3.Distance(
                    GetBuilding(targetBuildingId).AveragePosition,
                    possibleDoor.transform.position
                );
                doorList.Add(new WeightedItem<Door>(possibleDoor, 1f / distance));
            }

            return WeightedItem<Door>.Choose(doorList);
        }

        public Door GetFinishingDoorByTargetBuilding(Door selectedDoor, int targetBuildingId)
        //    TODO: Move this to DoorManager
        {
            List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();

            foreach (var possibleDoor in GetBuilding(targetBuildingId).Doors)
            {
                var distance = Vector3.Distance(selectedDoor.transform.position, possibleDoor.transform.position);
                doorList.Add(new WeightedItem<Door>(possibleDoor, Mathf.Pow(1 / distance, 10)));
            }

            return WeightedItem<Door>.Choose(doorList);
        }

        public BuildingInfo[] GetBuildingInfos()
        {
            return buildingInfoMap.Values.ToArray();
        }
    }
}