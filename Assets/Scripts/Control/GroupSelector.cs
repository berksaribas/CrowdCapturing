using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Simulation;
using UnityEngine;

namespace Control
{
    public class GroupSelector : MonoBehaviour
    {
        public GroupManager GroupManager;
        public AgentSelector AgentSelector;
        
        private readonly List<Action<GroupSequence>> listeners = new List<Action<GroupSequence>>();
        
        [CanBeNull]
        private GroupSequence focusedGroup = null;
        
        public GroupSequence FocusedGroup
        {
            get => focusedGroup;

            set
            {
                if (value != focusedGroup)
                {
                    focusedGroup = value;
                
                    foreach (var listener in listeners)
                    {
                        listener.Invoke(focusedGroup);
                    }
                }
            }
        }

        void Start()
        {
            AgentSelector.Observe(newFocus =>
            {
                if (newFocus != null && GroupManager.IsMemberOfAGroup(newFocus))
                {
                    FocusedGroup = GroupManager.GetActiveGroup(newFocus);
                }
                else
                {
                    FocusedGroup = null;
                }
            });
        }

        public void Observe(Action<GroupSequence> action)
        {
            listeners.Add(action);
        }
    }
}