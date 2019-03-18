using System.Collections.Generic;
using UnityEngine;
using World;

namespace Util
{
	public static class SequenceColorHelper
	{
		private static Dictionary<Building, Dictionary<Building, Color>> sequenceColors =
			new Dictionary<Building, Dictionary<Building, Color>>();

		public static Color GetColor(Building startingBuilding, Building targetBuilding)
		{
			if (!sequenceColors.ContainsKey(startingBuilding))
			{
				sequenceColors.Add(startingBuilding, new Dictionary<Building, Color>());
			}
			
			if (!sequenceColors[startingBuilding].ContainsKey(targetBuilding))
			{
				sequenceColors[startingBuilding]
					.Add(targetBuilding, Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f));
			}
				
			return sequenceColors[startingBuilding][targetBuilding];
		}
 	}
}