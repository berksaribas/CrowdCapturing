using System.Collections.Generic;
using System.IO;
using Simulation;
using UnityEngine;

namespace DefaultNamespace
{
	public class OverallData : MonoBehaviour
	{
		public static OverallData Instance { get; private set; }
		
		public Dictionary<int, int> PeopleLeavingBuilding = new Dictionary<int, int>();
		public Dictionary<int, int> ParentGroups = new Dictionary<int, int>();
		public Dictionary<int, int> ActivityByTime = new Dictionary<int, int>();
		public Dictionary<int, List<int>> WaitTimeByGroupSize = new Dictionary<int, List<int>>();
		public Dictionary<int, List<List<int>>> SubgroupsByGroupSize = new Dictionary<int, List<List<int>>>();

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

		public void AddWaitTime(int groupCount, int waitTime)
		{
			if (!WaitTimeByGroupSize.ContainsKey(groupCount))
			{		
				WaitTimeByGroupSize[groupCount] = new List<int>();
			}
			
			WaitTimeByGroupSize[groupCount].Add(waitTime);
		}
		
		public void AddSubgroupSize(int groupCount, List<int> subgroupSize)
		{
			if (!SubgroupsByGroupSize.ContainsKey(groupCount))
			{		
				SubgroupsByGroupSize[groupCount] = new List<List<int>>();
			}
			
			SubgroupsByGroupSize[groupCount].Add(subgroupSize);
		}
		
		private void Update()
		{
			var time = (int) SimulationController.Instance.SimulationTime.TimeInSeconds;
			var agentCount = FindObjectsOfType<Agent>().Length;
			ActivityByTime[time] = agentCount;
			
			if (Input.GetKeyDown(KeyCode.T) || time > 86000)
			{
				ExportData();
				Application.Quit();
				UnityEditor.EditorApplication.isPlaying = false;
			}
		}

		private void ExportData()
		{
			string path = "activity_data.csv";

			StreamWriter writer = new StreamWriter(path, false);
			foreach (var keyValuePair in ActivityByTime)
			{
				writer.WriteLine($"{keyValuePair.Key};{keyValuePair.Value}");
			}
			writer.Close();
			//////////////////////
			//////////////////////
			//////////////////////
			path = "group_sizes.csv";
			writer = new StreamWriter(path, false);
			foreach (var keyValuePair in ParentGroups)
			{
				writer.WriteLine($"{keyValuePair.Key};{keyValuePair.Value}");
			}
			writer.Close();
			//////////////////////
			//////////////////////
			//////////////////////
			path = "door_exit_group_sizes.csv";
			writer = new StreamWriter(path, false);
			foreach (var keyValuePair in PeopleLeavingBuilding)
			{
				writer.WriteLine($"{keyValuePair.Key};{keyValuePair.Value}");
			}
			writer.Close();
			//////////////////////
			//////////////////////
			//////////////////////
			path = "wait_times.csv";
			writer = new StreamWriter(path, false);
			foreach (var keyValuePair in WaitTimeByGroupSize)
			{
				writer.Write($"{keyValuePair.Key}");
				foreach (var i in keyValuePair.Value)
				{
					writer.Write($";{i}");
				}
				writer.WriteLine();
			}
			writer.Close();
			//////////////////////
			//////////////////////
			//////////////////////
			foreach (var keyValuePair in SubgroupsByGroupSize)
			{
				path = $"subgroups_{keyValuePair.Key}.csv";
				writer = new StreamWriter(path, false);

				foreach (var subgroupList in keyValuePair.Value)
				{
					foreach (var i in subgroupList)
					{
						writer.Write($";{i}");
					}
					
					writer.WriteLine();
				}
				
				writer.Close();
			}
		}
	}
}