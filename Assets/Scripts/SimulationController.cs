using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DefaultNamespace;
using Newtonsoft.Json;
using Simulation;
using UnityEngine;
using World;
using Random = UnityEngine.Random;

public class SimulationController : MonoBehaviour
{
	public static SimulationController Instance { get; private set; }

	public SequenceManager SequenceManager;
	public CrowdManager CrowdManager;
	public SimulationManager SimulationManager;
	public GroupManager GroupManager;
	public Building[] Buildings;
	
	public struct BuildingInfo
	{
		public Building Building;
		public int OrderInData;
		public float[] Weights;
	}
	
	[HideInInspector]
	public Dictionary<string, BuildingInfo> BuildingInfoMap;
	
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
		
		ReadSequenceCounts();
	}

	private void Start()
	{
		var mytxtData = (TextAsset) Resources.Load("json2");

		AgentJSONData[] agents = JsonConvert.DeserializeObject<AgentJSONData[]>(mytxtData.text);

		var agentsAndSequences = new Dictionary<int, List<Sequence>>(agents.Length);
		foreach (var agent in agents)
		{
			ConvertAgentDataToSequence(agent, agentsAndSequences);
		}
		
		CrowdManager.GenerateAgents(agentsAndSequences);
	}

	private void ReadSequenceCounts()
	{
		var csv = (TextAsset) Resources.Load("sequence_counts_matrix");

		var lines = csv.text.Trim().Split('\n');

		var buildingAliases = lines[0].Trim().Split(',')
			.Select(alias => alias.Trim('"'))
			.ToArray();

		var sequenceWeights = new Dictionary<string, float[]>(buildingAliases.Length);
		
		for (var i = 1; i < lines.Length; i++)
		{
			sequenceWeights[buildingAliases[i - 1]] = lines[i]
				.Trim()
				.Split(',')
				.Select(weight => float.Parse(weight, CultureInfo.InvariantCulture))
				.ToArray();
		}
		
		BuildingInfoMap = new Dictionary<string, BuildingInfo>(Buildings.Length);
		
		foreach (var building in Buildings)
		{
			BuildingInfoMap.Add(building.DataAlias, new BuildingInfo
			{
				Building = building,
				OrderInData = Array.IndexOf(buildingAliases, building.DataAlias),
				Weights = sequenceWeights[building.DataAlias]
			});
		}
	}
	
	private void ConvertAgentDataToSequence(AgentJSONData agent, Dictionary<int, List<Sequence>> agentsAndSequences)
	{
		var sequences = new List<Sequence>();
		agent.sequences.Sort();

		for (var index = 0; index < agent.sequences.Count - 1; index++)
		{
			var agentSequence = agent.sequences[index];
			var nextAgentSequence = agent.sequences[index + 1];

			var startTime = agentSequence.endDate.Split(' ')[1].Split(':');
			var startTimeSeconds = int.Parse(startTime[0]) * 3600 + int.Parse(startTime[1]) * 60 + Random.Range(0, 60);

			var startBuildingAlias = agentSequence.alias;
			var targetBuildingAlias = nextAgentSequence.alias;

			if (BuildingInfoMap.ContainsKey(startBuildingAlias) && BuildingInfoMap.ContainsKey(targetBuildingAlias))
			{
				var startingBuilding = BuildingInfoMap[startBuildingAlias].Building;
				var targetBuilding = BuildingInfoMap[targetBuildingAlias].Building;

				var sequence = new Sequence(
					int.Parse(agent.deviceId),
					startingBuilding,
					targetBuilding,
					startTimeSeconds
				);
				
				foreach (var id in nextAgentSequence.groupsWith)
				{
					sequence.AddGroupingAgent(id);
				}
				
				SequenceManager.InsertSequence(sequence);
				sequences.Add(sequence);
			}
		}

		agentsAndSequences.Add(int.Parse(agent.deviceId), sequences);
	}
}