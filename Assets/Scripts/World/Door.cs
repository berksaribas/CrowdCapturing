using UnityEngine;

namespace World
{
	public class Door : MonoBehaviour
	{
		public int Capacity = 3;
		public float EnterDurationInSeconds = 1.5f;
		
		public float[] lastEnterTimes;
		
		private void Awake()
		{
			Capacity = 3;
			EnterDurationInSeconds = 0.5f;
				
			lastEnterTimes = new float[Capacity];

			for (var i = 0; i < lastEnterTimes.Length; i++)
			{
				lastEnterTimes[i] = 0f;
			}
		}

		public bool IsDoorAvailable()
		{
			var currentTime = SimulationController.Instance.SimulationManager.WorldTimeSeconds;

			for (var index = 0; index < lastEnterTimes.Length; index++)
			{
				var lastEnterTime = lastEnterTimes[index];

				if (lastEnterTime + EnterDurationInSeconds < currentTime)
				{
					lastEnterTimes[index] = currentTime;
					return true;
				}
			}

			return false;
		}
	}
}