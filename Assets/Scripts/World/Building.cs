using System;
using System.Collections.Generic;
using Simulation;
using UnityEngine;

namespace World
{
	public class Building : MonoBehaviour
	{
		public string DataAlias;
		[NonSerialized] public Door[] Doors;
		[HideInInspector] public Vector3 AveragePosition;
		
		public readonly HashSet<Agent> AgentsInside = new HashSet<Agent>();

		private void Awake()
		{
			Doors = GetComponentsInChildren<Door>();
			foreach (var door in Doors)
				AveragePosition += door.transform.position;

			AveragePosition /= Doors.Length;
		}
	}
}