using System;
using UnityEngine;
using Util;
using World;
using Random = UnityEngine.Random;

namespace Simulation
{
    public class Sequence : IComparable<Sequence>
    {
        public readonly Building StartingBuilding, TargetBuilding;
        public int TotalOutput;
        public readonly int StartTime, Duration, CrowdSize;
        public readonly EasingFunction.Ease Ease;
        public float ProcessedTime = 0f;

        public float OutputCrowd;

        public readonly MaterialPropertyBlock ActorMaterialProperty;

        public Sequence(Building startingBuilding, Building targetBuilding, int crowdSize, int startTime, int duration, EasingFunction.Ease ease = EasingFunction.Ease.Linear)
        {
            StartingBuilding = startingBuilding;
            TargetBuilding = targetBuilding;
            CrowdSize = crowdSize;
            StartTime = startTime;
            Duration = duration;
            Ease = ease;

            ActorMaterialProperty = new MaterialPropertyBlock();
            var randomColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 0.8f, 1.0f);
            ActorMaterialProperty.SetColor("_Color", randomColor);
        }

        public int CompareTo(Sequence other)
        {
            return StartTime.CompareTo(other.StartTime);
        }
    }
}