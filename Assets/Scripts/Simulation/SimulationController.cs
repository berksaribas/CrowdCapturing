using System;
using System.Collections.Generic;
using System.Linq;
using JSONDataClasses;
using Newtonsoft.Json;
using UI;
using UI.Popup;
using UnityEngine;
using BuildingManager = World.BuildingManager;
using Random = UnityEngine.Random;

namespace Simulation
{
    public class SimulationController : MonoBehaviour
    {
        public static SimulationController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                Initialize();
            }
        }

        public Agent AgentPrefab;

        public TimeManager TimeManager;
        public BuildingManager BuildingManager;
        public MeetingManager MeetingManager;

        public TextAsset DailyData;

        private void Initialize()
        {
            UIState.Popup.Set(new Popup.Data("Loading Agent Data", "Please wait..."));

            var agentsData = JsonConvert.DeserializeObject<AgentData[]>(DailyData.text);

            ValidateData(ref agentsData);

            TimeManager.Initialize(agentsData);

            CreateSequencesAndInitializeAgents(agentsData);

            UIState.Popup.Set(null);
        }

        private void ValidateData(ref AgentData[] agentsData)
        {
            // Remove agents without enough connections
            agentsData = agentsData.Where(agent => agent.Connections.Length > 1).ToArray();

            // Sort connections with respect to their time
            foreach (var agent in agentsData)
                Array.Sort(agent.Connections);

            // Merge concurrent connections with the same building
            foreach (var agent in agentsData)
            {
                var connections = new List<ConnectionData>(agent.Connections.Length);

                connections.Add(agent.Connections[0]);

                foreach (var connection in agent.Connections)
                {
                    var latestConnection = connections.Last();

                    if (connection.BuildingAlias == latestConnection.BuildingAlias)
                    {
                        // Merge with the latest
                        latestConnection.EndTime = connection.EndTime;
                        latestConnection.GroupsWith = latestConnection.GroupsWith
                            .Intersect(connection.GroupsWith).ToArray();
                    }
                    else
                    {
                        // Add as a new connection
                        connections.Add(connection);
                    }
                }

                agent.Connections = connections.ToArray();
            }

            /*
            // Remove connections that start before the starting time
            var startTime = $"{TimeManager.StartHour:D2}:{TimeManager.StartMinute:D2}:00";
            foreach (var agent in agentsData)
            {
                // Find the index of first connection that ends after the startTime 
                var i = 0;
                for (; i < agent.Connections.Length; i++)
                    if (string.CompareOrdinal(agent.Connections[i].EndTime, startTime) >= 0)
                        break;

                // if all connections occur after start time
                if (i == agent.Connections.Length)
                    continue;

                // Connections before i should be removed and ith connection should be modified
                if (string.CompareOrdinal(agent.Connections[i].StartTime, startTime) < 0)
                    agent.Connections[i].StartTime = startTime;

                agent.Connections = agent.Connections.Skip(i).ToArray();
            }
            */

            // Validate connections' building aliases
            foreach (var agent in agentsData)
            {
                var validConnections = new List<ConnectionData>(agent.Connections.Length);

                for (var i = 0; i < agent.Connections.Length - 1; i++)
                {
                    var startBuildingAlias = agent.Connections[i].BuildingAlias;
                    var targetBuildingAlias = agent.Connections[i + 1].BuildingAlias;

                    if (BuildingManager.HasBuilding(startBuildingAlias) &&
                        BuildingManager.HasBuilding(targetBuildingAlias))
                        validConnections.Add(agent.Connections[i]);
                }

                agent.Connections = validConnections.ToArray();
            }

            // Remove agents without enough connections
            agentsData = agentsData.Where(agent => agent.Connections.Length > 1).ToArray();
        }

        private void CreateSequencesAndInitializeAgents(in AgentData[] agentsData)
        {
            var idAgentMap = new Dictionary<int, Agent>(agentsData.Length);

            foreach (var agentData in agentsData)
                idAgentMap[agentData.Id] = Instantiate(AgentPrefab, transform);

            foreach (var agentData in agentsData)
            {
                var sequences = new Sequence[agentData.Connections.Length - 1];

                for (var i = 0; i < agentData.Connections.Length - 1; i++)
                {
                    var connectionData = agentData.Connections[i];
                    var nextConnectionData = agentData.Connections[i + 1];

                    sequences[i] = new Sequence(
                        BuildingManager.GetBuilding(connectionData.BuildingAlias),
                        BuildingManager.GetBuilding(nextConnectionData.BuildingAlias),
                        connectionData.EndTime.TotalSeconds + Random.Range(0f, 60f), // +Noise
                        connectionData.GroupsWith
                            .Intersect(nextConnectionData.GroupsWith)
                            .Where(idAgentMap.ContainsKey)
                            .Select(id => idAgentMap[id])
                            .ToArray()
                    );
                }

                idAgentMap[agentData.Id].Initialize(agentData.Id, sequences);
            }

            Debug.Log($"Initialized {agentsData.Length} agents");
        }
    }
}