using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UI.State
{
    public static class FocusedCamera
    {
        private static readonly List<Action<Camera>> Listeners = new List<Action<Camera>>();

        [CanBeNull] private static Camera focus;

        public static Camera Get()
        {
            return focus;
        }

        public static void Set(Camera newFocus)
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

        public static void Observe(Action<Camera> action)
        {
            Listeners.Add(action);
        }
    }
}