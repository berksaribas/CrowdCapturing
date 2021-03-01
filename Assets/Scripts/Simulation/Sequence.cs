using System;
using JetBrains.Annotations;
using World;

namespace Simulation
{
    [Serializable]
    public class Sequence : IComparable<Sequence>
    {
        public readonly Building StartingBuilding, TargetBuilding;
        public readonly Door StartingDoor, TargetDoor;
        public readonly double StartTime;
        public readonly Agent[] PossibleGroupingAgents;
        [CanBeNull] public Meeting Meeting;

        public Sequence(Building startingBuilding, Building targetBuilding, double startTime, Agent[] possibleGroupingAgents)
        {
            StartingBuilding = startingBuilding;
            TargetBuilding = targetBuilding;
            
            StartTime = startTime;

            (StartingDoor, TargetDoor) = SimulationController.Instance.BuildingManager
                .FindDoorsFor(startingBuilding, targetBuilding);

            PossibleGroupingAgents = possibleGroupingAgents;
        }
        
        public int CompareTo(Sequence other)
        {
            return StartTime.CompareTo(other.StartTime);
        }
    }
}