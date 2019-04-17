using System;
using System.Collections.Generic;
using UnityEngine;
using Util;
using World;
using Random = UnityEngine.Random;

namespace Simulation
{
    public class Sequence : IComparable<Sequence>
    {
        public readonly int AgentId;
        public readonly Building StartingBuilding, TargetBuilding;
        public readonly int StartTime;
        public readonly MaterialPropertyBlock ActorMaterialProperty;
        public readonly List<int> GroupingAgents;

        public Sequence(int agentId, Building startingBuilding, Building targetBuilding, int startTime)
        {
            AgentId = agentId;
            StartingBuilding = startingBuilding;
            TargetBuilding = targetBuilding;
            StartTime = startTime;

            ActorMaterialProperty = new MaterialPropertyBlock();
            var randomColor = SequenceColorHelper.GetColor(startingBuilding, targetBuilding);
            ActorMaterialProperty.SetColor("_Color", randomColor);
            
            GroupingAgents = new List<int>();
        }

        public void AddGroupingAgent(int id)
        {
            GroupingAgents.Add(id);
        }
        
        public int CompareTo(Sequence other)
        {
            return StartTime.CompareTo(other.StartTime);
        }

        protected bool Equals(Sequence other)
        {
            return AgentId == other.AgentId && StartTime == other.StartTime;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Sequence) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AgentId * 397) ^ StartTime;
            }
        }
    }
}