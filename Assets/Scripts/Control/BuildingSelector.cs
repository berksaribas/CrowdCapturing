using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using World;

namespace Control
{
    public class BuildingSelector : MonoBehaviour
    {
        public CameraHandler CameraHandler;
        
        private readonly List<Action<Building>> listeners = new List<Action<Building>>();

        [CanBeNull]
        private Building focusedBuilding = null;
        
        public Building FocusedBuilding
        {
            get => focusedBuilding;

            set
            {
                if (value != focusedBuilding)
                {
                    focusedBuilding = value;
                
                    foreach (var listener in listeners)
                    {
                        listener.Invoke(focusedBuilding);
                    }
                }
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(1))
                return;

            var ray = CameraHandler.ActiveCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask("Buildings")))
            {
                FocusedBuilding = hit.transform.gameObject.GetComponent<Building>();
            }
            else
            {
                FocusedBuilding = null;
            }
        }
        
        public void Observe(Action<Building> action)
        {
            listeners.Add(action);
        }
    }
}