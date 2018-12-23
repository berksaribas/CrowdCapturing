using UnityEngine;
using World;

namespace Simulation
{	
	public class SimulationController : MonoBehaviour
	{
		public float WorldSpeed = 1.0f;
		public float WorldTimeSeconds = 0;

		private float lastRecordedTime = 0f;

		private void Update()
		{
			WorldTimeSeconds += (Time.realtimeSinceStartup - lastRecordedTime) * WorldSpeed;
			lastRecordedTime = Time.realtimeSinceStartup;
		}
		
		public void GenerateCrowds(Sequence sequence)
		{
			var crowdPerSecond = (float) sequence.CrowdSize / sequence.Duration;

			var startingDoor = sequence.StartingBuilding.GetDoorByTargetBuilding(sequence.TargetBuilding);
			var finishingDoor = sequence.TargetBuilding.GetRandomDoor();
			
			for (var i = 0; i < crowdPerSecond; i++)
			{
				CrowdManager.Instance.CreateAgent(startingDoor, finishingDoor);
			}
		}
	}
}