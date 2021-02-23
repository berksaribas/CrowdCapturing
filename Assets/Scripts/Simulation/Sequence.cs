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
        public readonly int StartTimeInSeconds;
        public readonly Agent[] PossibleGroupingAgents;
        [CanBeNull] public Meeting Meeting;

        public Sequence(Building startingBuilding, Building targetBuilding, int startTimeInSeconds, Agent[] possibleGroupingAgents)
        {
            StartingBuilding = startingBuilding;
            TargetBuilding = targetBuilding;
            
            StartTimeInSeconds = startTimeInSeconds;

            (StartingDoor, TargetDoor) = SimulationController.Instance.BuildingManager
                .FindDoorsFor(startingBuilding, targetBuilding);

            PossibleGroupingAgents = possibleGroupingAgents;
        }
        
        public int CompareTo(Sequence other)
        {
            return StartTimeInSeconds.CompareTo(other.StartTimeInSeconds);
        }
    }
}