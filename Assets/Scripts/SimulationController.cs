using System.Collections.Generic;
using DefaultNamespace;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using Simulation;
using UnityEngine;
using Util;
using World;

public class SimulationController : MonoBehaviour
{
	public static SimulationController Instance { get; private set; }

	public SequenceManager SequenceManager;
	public CrowdManager CrowdManager;
	public SimulationManager SimulationManager;
	public Building[] Buildings;

	private Dictionary<string, int> buildingIndexMap = new Dictionary<string, int>()
	{
		{"UM", 0},
		{"Yurt", 1},
		{"FENS", 2},
		{"FASS", 3},
		{"FMAN", 4},
		{"IC", 5},
	};

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

		TextAsset mytxtData = (TextAsset) Resources.Load("json");
		string txt = mytxtData.text;

		AgentJSONData[] agents = JsonConvert.DeserializeObject<AgentJSONData[]>(txt);

		var agentsAndSequences = new Dictionary<int, List<Sequence>>();
		for (var i = 1; i < agents.Length; i++)
		{
			ConvertAgentDataToSequence(agents[i], agentsAndSequences);
		}

		CrowdManager.GenerateAgents(agentsAndSequences);
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

			if (buildingIndexMap.ContainsKey(startBuildingAlias) && buildingIndexMap.ContainsKey(targetBuildingAlias))
			{
				var startingBuilding = Buildings[buildingIndexMap[startBuildingAlias]];
				var targetBuilding = Buildings[buildingIndexMap[targetBuildingAlias]];

				var sequence = new Sequence(int.Parse(agent.deviceId), startingBuilding, targetBuilding,
					startTimeSeconds);
				SequenceManager.InsertSequence(sequence);
				sequences.Add(sequence);
//				Debug.Log(
//					$"Inserting a sequence for the agent {agent.deviceId} with {startingBuilding} at time {startTimeSeconds}");
			}
		}

		agentsAndSequences.Add(int.Parse(agent.deviceId), sequences);
	}
}