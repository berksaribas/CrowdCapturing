using System;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace Simulation
{
    public class DoorManager : MonoBehaviour
    {
        public GameObject BuildingsParent;

        [HideInInspector] public Door[] Doors;
        private Dictionary<Door, Queue<AgentData>> doorAgentQueue;

        private void Awake()
        {
            Doors = BuildingsParent.GetComponentsInChildren<Door>();

            doorAgentQueue = new Dictionary<Door, Queue<AgentData>>(Doors.Length);
            foreach (var door in Doors)
            {
                doorAgentQueue[door] = new Queue<AgentData>();
            }
        }
    }
}