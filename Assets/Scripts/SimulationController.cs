using System.Collections.Generic;
using DefaultNamespace;
using Newtonsoft.Json;
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
    private int counter = 0;
    private Dictionary<string, int> buildingIndexMap = new Dictionary<string, int>()
    {
        {"UM", 1},
        {"FENS", 2},
        {"FASS", 3},
        {"FMAN", 4},
        {"Yurt", 6},
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
        
        TextAsset mytxtData=(TextAsset)Resources.Load("json");
        string txt = mytxtData.text;
        
        AgentJSONData[] agents = JsonConvert.DeserializeObject<AgentJSONData[]>(txt);
        for (var i = 1; i < agents.Length; i++)
        {
            ConvertAgentDataToSequence(agents[i]);
        }
        
        Debug.Log(counter);
        
//        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[1], 30, 45000, 600)); //UC->UC 2
//        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[2], 80, 45000, 600,
//            EasingFunction.Ease.EaseOutExpo)); //UC->FENS
//        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[3], 130, 45000, 600,
//            EasingFunction.Ease.EaseOutExpo)); //UC->FASS
//        
//        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[1], 100, 45000, 600)); //UC 2->FMAN
//        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[5], 40, 45000, 600)); //UC 2->SL
//
//        SequenceManager.InsertSequence(new Sequence(Buildings[2], Buildings[0], 300, 45000, 600,
//            EasingFunction.Ease.EaseOutExpo)); //FENS->UC
//        SequenceManager.InsertSequence(new Sequence(Buildings[2], Buildings[3], 70, 45000, 600)); //FENS->FASS
//        
//        SequenceManager.InsertSequence(new Sequence(Buildings[3], Buildings[0], 330, 45000, 600,
//            EasingFunction.Ease.EaseOutExpo)); //FASS->UC
//        SequenceManager.InsertSequence(new Sequence(Buildings[3], Buildings[2], 120, 45000, 600)); //FASS->FENS
//        SequenceManager.InsertSequence(new Sequence(Buildings[3], Buildings[4], 60, 45000, 600)); //FASS->FMAN
//        
//        SequenceManager.InsertSequence(new Sequence(Buildings[4], Buildings[0], 50, 45000, 600)); //FMAN->UC
//        SequenceManager.InsertSequence(new Sequence(Buildings[4], Buildings[1], 130, 45000, 600)); //FMAN->UC 2

    }

    private void ConvertAgentDataToSequence(AgentJSONData agent)
    {
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
                
                SequenceManager.InsertSequence(new Sequence(startingBuilding, targetBuilding, 1, startTimeSeconds, 2));
                Debug.Log($"Inserting a sequence with {startingBuilding} at time {startTimeSeconds}");
                counter++;
            }
            
        }
    }
}