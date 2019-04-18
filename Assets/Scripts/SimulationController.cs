using System.Collections.Generic;
using DefaultNamespace;
using Newtonsoft.Json;
using Simulation;
using UnityEngine;
using World;

public class SimulationController : MonoBehaviour
{
	public static SimulationController Instance { get; private set; }

	public SequenceManager SequenceManager;
	public CrowdManager CrowdManager;
	public SimulationManager SimulationManager;
	public GroupManager GroupManager;
	public Building[] Buildings;

	private Dictionary<string, int> buildingIndexMap = new Dictionary<string, int>();
	
	private void Awake()
	{
		for (var index = 0; index < Buildings.Length; index++)
		{
			var building = Buildings[index];
			buildingIndexMap.Add(building.DataAlias, index);
		}

		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		var mytxtData = (TextAsset) Resources.Load("json2");
		var txt = mytxtData.text;

		AgentJSONData[] agents = JsonConvert.DeserializeObject<AgentJSONData[]>(txt);

		var agentsAndSequences = new Dictionary<int, List<Sequence>>();
		foreach (var agent in agents)
		{
			ConvertAgentDataToSequence(agent, agentsAndSequences);
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