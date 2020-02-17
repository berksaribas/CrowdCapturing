using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Simulation;

namespace UI.State
{
    public class FocusedGroup
    {
        private static readonly List<Action<GroupSequence>> Listeners = new List<Action<GroupSequence>>();

        [CanBeNull] private static GroupSequence focus;

        public static GroupSequence Get()
        {
            return focus;
        }

        public static void Set(GroupSequence newFocus)
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

        public static void Observe(Action<GroupSequence> action)
        {
            Listeners.Add(action);
        }
    }
}