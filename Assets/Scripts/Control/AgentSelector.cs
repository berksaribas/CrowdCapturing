using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Simulation;
using UnityEngine;

namespace Control
{
    public class AgentSelector : MonoBehaviour
    {
        private readonly List<Action<Agent>> listeners = new List<Action<Agent>>();
        
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
                
                    foreach (var listener in listeners)
                    {
                        listener.Invoke(focusedAgent);
                    }
                }
                
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask("Agents")))
            {
                FocusedAgent = hit.transform.gameObject.GetComponent<Agent>();
            }
            else
            {
                FocusedAgent = null;
            }
        }

        public void Observe(Action<Agent> action)
        {
            listeners.Add(action);
        }
    }
}