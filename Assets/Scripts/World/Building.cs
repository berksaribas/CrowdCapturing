using System;
using UnityEngine;

namespace World
{
	public class Building : MonoBehaviour
	{
		public string DataAlias;
		[NonSerialized] public Door[] Doors;
		[HideInInspector] public Vector3 AveragePosition;

		private void Awake()
		{
			Doors = GetComponentsInChildren<Door>();
			foreach (var door in Doors)
			{
				AveragePosition += door.transform.position;
			}

			AveragePosition /= Doors.Length;
		}
	}
}