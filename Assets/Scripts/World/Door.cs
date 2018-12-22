using UnityEngine;

namespace World
{
	public class Door : MonoBehaviour
	{
		public Building[] TargetBuildings;
		public float[] UsagePercentages;

		private void Awake()
		{
			var sum = 0f;
			foreach (var usagePercentage in UsagePercentages)
			{
				sum += usagePercentage;
			}
			
			for (var i = 0; i < UsagePercentages.Length; i++)
			{
				UsagePercentages[i] = UsagePercentages[i] / sum;
			}
		}

		public float GetNormalizedUsagePercentage(Building building)
		{
			var i = 0;
			for (; i < TargetBuildings.Length; i++)
			{
				if (TargetBuildings[i] == building)
				{
					break;
				}
			}

			return UsagePercentages[i];
		}
	}
}