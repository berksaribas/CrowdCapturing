using System;
using Util;
using World;

namespace Simulation
{
	public class Sequence : IComparable<Sequence>
	{
		public Building StartingBuilding, TargetBuilding;
		public int CrowdSize, TotalOutput;
		public int StartTime, Duration;
		public EasingFunction.Ease Ease;
		public float ProcessedTime = 0f;

		public float OutputCrowd;

		public Sequence(Building startingBuilding, Building targetBuilding, int crowdSize, int startTime, int duration, EasingFunction.Ease ease = EasingFunction.Ease.Linear)
		{
			StartingBuilding = startingBuilding;
			TargetBuilding = targetBuilding;
			CrowdSize = crowdSize;
			StartTime = startTime;
			Duration = duration;
			Ease = ease;
		}

		public int CompareTo(Sequence other)
		{
			return StartTime.CompareTo(other.StartTime);
		}
	}
}