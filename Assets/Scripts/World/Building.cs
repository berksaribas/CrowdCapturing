using UnityEngine;

namespace World
{
	public class Building : MonoBehaviour
	{
		public string DataAlias;
		public Door[] Doors;
		[HideInInspector] public Vector3 AveragePosition;

		private void Awake()
		{			
			foreach (var door in Doors)
			{
				AveragePosition += door.transform.position;
			}

			AveragePosition /= Doors.Length;
		}
	}
}