using System.Collections.Generic;
using Simulation;
using UnityEngine;

namespace UI.InGame
{
    public class GroupHighlighter : MonoBehaviour
    {
        private Agent[] focusedGroup;
        
        public MeshRenderer HighlighterObject;
        
        private readonly List<GameObject> memberHighlighters = new List<GameObject>(12);
        
        void Start()
        {
            UIState.Group.OnChange += newGroup =>
            {
                memberHighlighters.ForEach(Destroy);
                memberHighlighters.Clear();
        
                if ((focusedGroup = newGroup) != null)
                {
                    HighlighterObject.enabled = true;
        
                    foreach (var agent in focusedGroup)
                        memberHighlighters.Add(
                            Instantiate(HighlighterObject.gameObject, agent.transform, false)
                        );
        
                    HighlighterObject.enabled = false;
                }
            };
        }
    }
}