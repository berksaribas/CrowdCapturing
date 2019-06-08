using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public class OverallData : MonoBehaviour
	{
		public static OverallData Instance { get; private set; }
		public Dictionary<int, int> PeopleLeavingBuilding = new Dictionary<int, int>();
		public Dictionary<int, int> ParentGroups = new Dictionary<int, int>();

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
			}
			else
			{
				Instance = this;
			}
		}

		public void AddAgentsLeavingBuilding(int groupCount)
		{
			if (!PeopleLeavingBuilding.ContainsKey(groupCount))
			{
				PeopleLeavingBuilding[groupCount] = 0;
			}
			
			PeopleLeavingBuilding[groupCount]++;
		}
		
		public void AddParentGroup(int groupCount)
		{
			if (!ParentGroups.ContainsKey(groupCount))
			{
				ParentGroups[groupCount] = 0;
			}
			
			ParentGroups[groupCount]++;
		}
	}
}