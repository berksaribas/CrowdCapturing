using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DefaultNamespace;
using Newtonsoft.Json;
using Simulation;
using UnityEngine;
using Util;
using World;
using Random = UnityEngine.Random;

public class SimulationController : MonoBehaviour
{
	public static SimulationController Instance { get; private set; }

	public SequenceManager SequenceManager;
	public AgentManager AgentManager;
	public SimulationTime SimulationTime;
	public GroupManager GroupManager;
	public BuildingManager BuildingManager;
	public DoorManager DoorManager;
	
	public Baker AgentsAndSequencesBaker = new Baker(async component =>
	{
		var self = component as SimulationController;

		self.BuildingManager.Awake();
		
		var jsonData = Resources.Load("29092016") as TextAsset;

		AgentJSONData[] agents = JsonConvert.DeserializeObject<AgentJSONData[]>(jsonData.text);

		var agentsAndSequences = new Dictionary<int, List<Sequence>>(agents.Length);
		foreach (var agent in agents)
		{
			self.ConvertAgentDataToSequence(agent, agentsAndSequences);
		}

		return agentsAndSequences;
	});

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

	private void Start()
	{
		var agentsAndSequences =
			AgentsAndSequencesBaker.LoadBaked<Dictionary<int, List<Sequence>>>();

		AgentManager.InstantiateAgents(
			agentsAndSequences
		);

		foreach (var sequences in agentsAndSequences.Values)
		{
			SequenceManager.InsertSequences(sequences);
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

			if (BuildingManager.HasBuilding(startBuildingAlias) && BuildingManager.HasBuilding(targetBuildingAlias))
			{
				var sequence = new Sequence(
					int.Parse(agent.deviceId),
					BuildingManager.GetBuildingId(startBuildingAlias),
					BuildingManager.GetBuildingId(targetBuildingAlias),
					startTimeSeconds
				);
				
				foreach (var id in nextAgentSequence.groupsWith)
				{
					sequence.AddGroupingAgent(id);
				}
				
				sequences.Add(sequence);
			}
		}

		agentsAndSequences.Add(int.Parse(agent.deviceId), sequences);
	}
}