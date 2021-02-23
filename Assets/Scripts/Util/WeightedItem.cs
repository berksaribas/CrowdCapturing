using System;
using System.Collections.Generic;
using System.Linq;

namespace Util
{
	public class WeightedItem<T>
	{
		private static readonly Random rndInst = new Random();
		
		private T value;
		private float weight;
		private float cumulativeSum;

		public WeightedItem(T value, float weight)
		{
			this.value = value;
			this.weight = weight;
		}

		public static T PickWeightedRandom(List<WeightedItem<T>> items)
		{
			float cumulativeSum = 0;
			var count = items.Count();

			for (var slot = 0; slot < count; slot++)
			{
				cumulativeSum += items[slot].weight;
				items[slot].cumulativeSum = cumulativeSum;
			}

			var divSpot = rndInst.NextDouble() * cumulativeSum;
			var chosen =  items.FirstOrDefault(i => i.cumulativeSum >= divSpot);
			
			if (chosen == null)
				throw new Exception("No item chosen - there seems to be a problem with the probability distribution.");
			
			return chosen.value;
		}
	}
}