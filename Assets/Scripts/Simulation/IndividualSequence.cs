using System;
using World;

namespace Simulation
{
    public class IndividualSequence : IComparable<IndividualSequence>
    {
        public Building StartingBuilding, TargetBuilding;
        private float StartTime;

        public IndividualSequence(Building startingBuilding, Building targetBuilding, float startTime)
        {
            StartingBuilding = startingBuilding;
            TargetBuilding = targetBuilding;
            StartTime = startTime;
        }

        public int CompareTo(IndividualSequence other)
        {
            return StartTime.CompareTo(other.StartTime);
        }
    }
}