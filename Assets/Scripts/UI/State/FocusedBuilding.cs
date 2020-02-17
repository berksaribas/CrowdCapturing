using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using World;

namespace UI.State
{
    public static class FocusedBuilding
    {
        private static readonly List<Action<Building>> Listeners = new List<Action<Building>>();

        [CanBeNull] private static Building focus;

        public static Building Get()
        {
            return focus;
        }

        public static void Set(Building newFocus)
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

        public static void Observe(Action<Building> action)
        {
            Listeners.Add(action);
        }
    }
}