using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation
{
    public class MeetingManager : MonoBehaviour
    {
        public float MeetAtDoorThreshold = 80f;
        public int MaxWaitTimeForGroupMembers = 600;
        
        // Each frame every agent that starts its sequence will registers in this dictionary with their activated sequence
        private readonly Dictionary<Agent, Sequence> registeredAgents = new Dictionary<Agent, Sequence>();
        public void RegisterForMeeting(Agent agent, Sequence sequence) => registeredAgents.Add(agent, sequence);

        // Calculate the meetings of the registered agents
        private void LateUpdate()
        {
            if (registeredAgents.Count == 0)
                return;
            
            Debug.Log($"Calculating groups for {registeredAgents.Count} agents [{string.Join(", ", registeredAgents.Select(a => a.Key.Id))}]");
            var groups = new List<HashSet<Agent>>();
            foreach (var keyValue in registeredAgents)
            {
                var agent = keyValue.Key;
                var sequence = keyValue.Value;

                // Check if this agent is already put in a group
                if (groups.Any(g => g.Contains(agent)))
                    continue;

                var group = new HashSet<Agent> {agent};
                foreach (var possibleGroupingAgent in sequence.PossibleGroupingAgents)
                {
                    var s = possibleGroupingAgent.NextSequence;
                    var agentCanGroup =
                        s != null
                        && s.StartTimeInSeconds - sequence.StartTimeInSeconds <= 600
                        && s.StartingBuilding == sequence.StartingBuilding
                        && s.PossibleGroupingAgents.Contains(agent);
                    
                    if (agentCanGroup)
                        group.Add(possibleGroupingAgent);
                }
                groups.Add(group);
            }
            registeredAgents.Clear();

            Debug.Log($"Calculating meetings for {groups.Count} groups [{string.Join(" | ", groups.Select(g => string.Join(", ", g.Select(a => a.Id))))}]");
            foreach (var group in groups)
            {
                // TODO: Create meetings
                //sequence.Meeting = null;
            }
        }
    }
}