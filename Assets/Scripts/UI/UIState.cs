using Simulation;
using UnityEngine;
using Util;
using World;

namespace UI
{
    public static class UIState
    {
        public static readonly Observable<Camera> Camera = new Observable<Camera>();
        
        public static readonly Observable<Agent> Agent = new Observable<Agent>();
        public static readonly Observable<GroupSequence> Group = new Observable<GroupSequence>();
        
        public static readonly Observable<Building> Building = new Observable<Building>();
    }
}