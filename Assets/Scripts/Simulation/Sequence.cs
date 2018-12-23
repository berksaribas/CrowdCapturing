using System;
using World;

namespace Simulation
{
	public class Sequence : IComparable<Sequence>
	{
		public Building StartingBuilding, TargetBuilding;
		public int CrowdSize, RemainingCrowdSize;
		public int StartTime, Duration;

		public Sequence(Building startingBuilding, Building targetBuilding, int crowdSize, int startTime, int duration)
		{
			StartingBuilding = startingBuilding;
			TargetBuilding = targetBuilding;
			CrowdSize = crowdSize;
			StartTime = startTime;
			Duration = duration;
			RemainingCrowdSize = CrowdSize;
		}

		public int CompareTo(Sequence other)
		{
			return StartTime.CompareTo(other.StartTime);
		}
	}
}