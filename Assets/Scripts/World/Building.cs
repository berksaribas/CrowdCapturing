using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Simulation;
using UnityEngine;
using Util;

namespace World
{
	public class Building : MonoBehaviour
	{
		private List<Agent> agents = new List<Agent>();
		public Door[] Doors;
		[HideInInspector] public Vector3 AveragePosition;

		[Util.ReadOnly]
		public int AgentCount;

		private void Awake()
		{			
			foreach (var door in Doors)
			{
				AveragePosition += door.transform.position;
			}

			AveragePosition /= Doors.Length;
		}

		public Door GetDoorByTargetBuilding(Building building)
		{
			List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();

			foreach (var possibleDoor in Doors)
			{
				var distance = Vector3.Distance(building.AveragePosition, possibleDoor.transform.position);
				doorList.Add(new WeightedItem<Door>(possibleDoor, 1 / distance));
			}

			return WeightedItem<Door>.Choose(doorList);
		}

		public Door GetFinishingDoorByTargetBuilding(Door selectedDoor, Building building)
		{
			List<WeightedItem<Door>> doorList = new List<WeightedItem<Door>>();

			foreach (var possibleDoor in building.Doors)
			{
				var distance = Vector3.Distance(selectedDoor.transform.position, possibleDoor.transform.position);
				doorList.Add(new WeightedItem<Door>(possibleDoor, Mathf.Pow(1 / distance, 10)));
			}

			return WeightedItem<Door>.Choose(doorList);
		}

		public void RegisterAgent(Agent agent)
		{
			agents.Add(agent);
			AgentCount = agents.Count;
		}

		public void UnregisterAgent(Agent agent)
		{
			agents.Remove(agent);
			AgentCount = agents.Count;
		}
	}
}