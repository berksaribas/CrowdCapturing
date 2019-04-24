using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Simulation;
using UnityEngine;

namespace Control
{
    public class AgentAndGroupSelector : MonoBehaviour
    {
        public CameraHandler CameraHandler;

        private readonly List<Action<Agent>> agentListeners = new List<Action<Agent>>();
        
        [CanBeNull]
        private Agent focusedAgent = null;
        
        public Agent FocusedAgent
        {
            get => focusedAgent;

            set
            {
                if (value != focusedAgent)
                {
                    focusedAgent = value;
                
                    foreach (var listener in agentListeners)
                    {
                        listener.Invoke(focusedAgent);
                    }
                }
                
            }
        }
        
        private readonly List<Action<GroupSequence>> groupListeners = new List<Action<GroupSequence>>();
        
        [CanBeNull]
        private GroupSequence focusedGroup = null;

        public GroupManager GroupManager;
        
        public GroupSequence FocusedGroup
        {
            get => focusedGroup;

            set
            {
                if (value != focusedGroup)
                {
                    focusedGroup = value;
                
                    foreach (var listener in groupListeners)
                    {
                        listener.Invoke(focusedGroup);
                    }
                }
                
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = CameraHandler.ActiveCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask("Agents")))
            {
                FocusedAgent = hit.transform.gameObject.GetComponent<Agent>();
                FocusedGroup = GroupManager.GetActiveGroup(FocusedAgent);
            }
            else
            {
                FocusedAgent = null;
                FocusedGroup = null;
            }
        }

        public void ObserveAgent(Action<Agent> action)
        {
            agentListeners.Add(action);
        }
        
        public void ObserveGroup(Action<GroupSequence> action)
        {
            groupListeners.Add(action);
        }
    }
}