using System.Collections.Generic;
using UnityEngine;
using Util;

namespace World
{
	public class Building : MonoBehaviour
	{
		public Door[] doors;
		public Vector3 averagePosition;

		private void Awake()
		{
			foreach (var door in doors)
			{
				averagePosition += door.transform.position;
			}

			averagePosition /= doors.Length;
		}

		public Door GetDoorByTargetBuilding(Building building)
		{
			List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();

			foreach (var possibleDoor in doors)
			{
				var distance = Vector3.Distance(building.averagePosition, possibleDoor.transform.position);
				doorList.Add(new WeightedItem<Door>(possibleDoor, 1 / distance));
			}

			return WeightedItem<Door>.Choose(doorList);
		}

		public Door GetFinishingDoorByTargetBuilding(Door selectedDoor, Building building)
		{
			List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();

			foreach (var possibleDoor in building.doors)
			{
				var distance = Vector3.Distance(selectedDoor.transform.position, possibleDoor.transform.position);
				doorList.Add(new WeightedItem<Door>(possibleDoor, Mathf.Pow(1 / distance, 10)));
			}

			return WeightedItem<Door>.Choose(doorList);
		}
	}
}