using System.Collections.Generic;
using Simulation;
using UnityEngine;
using World;

namespace Util
{
    public static class SequenceColorHelper
    {
        private static readonly Dictionary<int, Dictionary<int, Color>> SequenceColors =
            new Dictionary<int, Dictionary<int, Color>>();

        public static Color GetColor(int startingBuildingId, int targetBuildingId)
        {
            if (!SequenceColors.ContainsKey(startingBuildingId))
            {
                SequenceColors.Add(startingBuildingId, new Dictionary<int, Color>());
            }

            if (!SequenceColors[startingBuildingId].ContainsKey(targetBuildingId))
            {
                SequenceColors[startingBuildingId]
                    .Add(targetBuildingId,
                        Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f));
            }

            return SequenceColors[startingBuildingId][targetBuildingId];
        }
    }
}