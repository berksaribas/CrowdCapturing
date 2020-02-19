using System;
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

        private void Awake()
        {
            UIState.Group.OnChange += SetGroupMembers;
        }

        private void OnGUI()
        {
            var group = UIState.Group.Get();
            if (group == null) return;

            ID.text = group.debugText;
            TargetDoor.text = group.TargetDoor.name;
            MeetingPoint.text = group.MeetingPoint.ToString();
        }

        private void SetGroupMembers(GroupSequence group)
        {
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
}