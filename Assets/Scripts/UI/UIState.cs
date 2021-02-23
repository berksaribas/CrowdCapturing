using Simulation;
using UnityEngine;
using Util;
using World;

namespace UI
{
    public static class UIState
    {
        public static readonly Observable<Popup.Popup.Data> Popup = new Observable<Popup.Popup.Data>();
     
        public static readonly Observable<Camera> Camera = new Observable<Camera>();
        
        public static readonly Observable<Agent> Agent = new Observable<Agent>();
        public static readonly Observable<Agent[]> Group = new Observable<Agent[]>();
        public static readonly Observable<PathTraverser[]> PathTraversers = new Observable<PathTraverser[]>();
        
        public static readonly Observable<Building> Building = new Observable<Building>();
    }
}