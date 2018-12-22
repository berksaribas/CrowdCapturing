using System.Collections.Generic;
using UnityEngine;
using Util;

namespace World
{
	public class Building : MonoBehaviour
	{
		public Door[] doors;
		private Dictionary<Building, List<Door>> buildingDoorPairs = new Dictionary<Building, List<Door>>();

		private void Awake()
		{
			foreach (var door in doors)
			{
				foreach (var targetBuilding in door.TargetBuildings)
				{
					if (!buildingDoorPairs.ContainsKey(targetBuilding))
					{
						buildingDoorPairs.Add(targetBuilding, new List<Door>());
					}

					buildingDoorPairs[targetBuilding].Add(door);
				}
			}
		}

		public Door GetDoorByTargetBuilding(Building building)
		{
			var possibleDoors = buildingDoorPairs[building];
			List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();

			foreach (var possibleDoor in possibleDoors)
			{
				doorList.Add(new WeightedItem<Door>(possibleDoor,
					(int)(possibleDoor.GetNormalizedUsagePercentage(building) * 100)));
			}

			return WeightedItem<Door>.Choose(doorList);
		}
	}
}