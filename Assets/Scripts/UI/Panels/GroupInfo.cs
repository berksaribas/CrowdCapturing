using System.Collections.Generic;
using Simulation;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class GroupInfo : MonoBehaviour
    {
        public TextMeshProUGUI ID, TargetDoor, MeetingPoint;

        public Transform GroupMemberList;
        public GameObject GroupMemberPrefab;
        private readonly List<AgentWithState> members = new List<AgentWithState>(5);
        
        private GroupSequence group;
        public GroupSequence Group
        {
            get => group;
            set
            {
                group = value;
                
                gameObject.SetActive(group != null);
                
                if (group == null)
                {
                    foreach (var member in members)
                    {
                        member.Agent = null;
                    }

                    return;
                }

                // Will run only if members.Count < group.agentCount
                for (var i = members.Count; i < group.agentCount; i++)
                {
                    members.Add(
                        Instantiate(GroupMemberPrefab, GroupMemberList)
                            .GetComponent<AgentWithState>()
                    );
                }

                // Set group agents to UI items
                for (var i = 0; i < group.agentCount; i++)
                {
                    members[i].Agent = group.agents[i];
                }

                // Set the rest inactive
                for (var i = group.agentCount; i < members.Count; i++)
                {
                    members[i].Agent = null;
                }
            }
        }

        private void Awake()
        {
            UIState.Group.OnChange += group => Group = group;
            Group = UIState.Group.Get();
        }

        private void OnGUI()
        {
            ID.text = group.debugText;
            TargetDoor.text = group.TargetDoor.name;
            MeetingPoint.text = group.MeetingPoint.ToString();
        }
    }
}