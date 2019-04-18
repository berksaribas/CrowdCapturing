using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Simulation;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Util;

namespace Control
{
    public class AgentSelector : MonoBehaviour
    {
        [CanBeNull] [NonSerialized] public Agent FocusedAgent = null;

        public GroupManager GroupManager;

        public Text IdleText, StaticText, DynamicText;

        public MeshRenderer AgentHighlighter;

        public GameObject PathHighlighterPrefab;
        private readonly List<GameObject> pathHighlighter = new List<GameObject>();

        private void Awake()
        {
            ResetCanvas();
        }

        private void Update()
        {
            if (FocusedAgent != null)
            {
                AgentHighlighter.transform.position = FocusedAgent.transform.position;
                UpdateCanvas();
            }

            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask("Agents")))
            {
                if (FocusedAgent != null)
                {
                    ClearPathHighlighter();
                }

                FocusedAgent = hit.transform.gameObject.GetComponent<Agent>();

                AgentHighlighter.enabled = true;
                ConfigurePathHighlighterTo(FocusedAgent);

                SetCanvas();
            }
            else
            {
                FocusedAgent = null;
                AgentHighlighter.enabled = false;
                ClearPathHighlighter();

                ResetCanvas();
            }
        }

        private void ResetCanvas()
        {
            IdleText.enabled = true;
            StaticText.enabled = false;
            DynamicText.enabled = false;
        }

        private void SetCanvas()
        {
            IdleText.enabled = false;
            StaticText.enabled = true;
            DynamicText.enabled = true;

            var text = new StringBuilder();

            StaticText.text = text.ToString();
        }

        private void UpdateCanvas()
        {
            var text = new StringBuilder();

            text.Append($"From: {FocusedAgent.GetStartingDoorName()}");
            text.Append($" @ {TimeHelper.ConvertSecondsToString(FocusedAgent.GetNextSequence().StartTime)}\n\n");

            if (GroupManager.IsMemberOfAGroup(FocusedAgent))
            {
                switch (FocusedAgent.State)
                {
                    case AgentState.WalkingToMeetingPosition:
                        text.Append("Walking to meeting position.\n");
                        break;

                    case AgentState.WaitingGroupMembers:
                        text.Append("Waiting for other group members.\n");
                        break;

                    case AgentState.WalkingToTargetDoor:
                        text.Append($"Walking to the door '{FocusedAgent.GetTargetDoorName()}' with a group.\n");
                        break;

                    default:
                        text.Append($"MISSING STATE STATUS! STATUS = {FocusedAgent.State}\n");
                        break;
                }
            }
            else
            {
                text.Append($"Walking to the door '{FocusedAgent.GetTargetDoorName()}'.\n");
            }

            DynamicText.text = text.ToString();
        }

        private void ClearPathHighlighter()
        {
            pathHighlighter.ForEach(Destroy);
            pathHighlighter.Clear();
        }

        private void ConfigurePathHighlighterTo(Agent agent)
        {
            var path = agent.GetComponent<NavMeshAgent>().path.corners;

            for (var i = 1; i < path.Length; i++)
            {
                pathHighlighter.Add(
                    HighlightLine.CreateNew(
                        path[i - 1],
                        path[i],
                        transform
                    )
                );
            }
        }
    }
}