using UnityEngine;
using World;

namespace Simulation
{	
	public class SimulationManager : MonoBehaviour
	{
		private static SimulationManager _instance;

		public static SimulationManager Instance { get { return _instance; } }

		public float WorldSpeed = 1.0f;
		public float WorldTimeSeconds = 0;

		private float lastRecordedTime = 0f;

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Destroy(this.gameObject);
			} else {
				_instance = this;
			}
		}

		private void Update()
		{
			WorldTimeSeconds += (Time.realtimeSinceStartup - lastRecordedTime) * WorldSpeed;
			lastRecordedTime = Time.realtimeSinceStartup;
		}
		
		public void GenerateCrowds(Sequence sequence)
		{
			var crowdPerSecond = (float) sequence.CrowdSize / sequence.Duration;

			var startingDoor = sequence.StartingBuilding.GetDoorByTargetBuilding(sequence.TargetBuilding);
			var finishingDoor =
				sequence.StartingBuilding.GetFinishingDoorByTargetBuilding(startingDoor, sequence.TargetBuilding);
			
			for (var i = 0; i < crowdPerSecond; i++)
			{
				CrowdManager.Instance.CreateAgent(startingDoor, finishingDoor);
			}
		}
	}
}