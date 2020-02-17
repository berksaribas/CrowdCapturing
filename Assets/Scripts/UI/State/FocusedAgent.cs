using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Simulation;

namespace UI.State
{
    public static class FocusedAgent
    {
        private static readonly List<Action<Agent>> Listeners = new List<Action<Agent>>();

        [CanBeNull] private static Agent focus;

        public static Agent Get()
        {
            return focus;
        }

        public static void Set(Agent newFocus)
        {
            if (newFocus != focus)
            {
                focus = newFocus;
                
                foreach (var listener in Listeners)
                {
                    listener.Invoke(focus);
                }
            }
        }

        public static void Observe(Action<Agent> action)
        {
            Listeners.Add(action);
        }
    }
}