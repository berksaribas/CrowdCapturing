using System.Collections.Generic;
using System.Linq;
using Control;
using Simulation;
using UnityEngine;

namespace UI
{
    public class GroupHighlighter : MonoBehaviour
    {
        public AgentAndGroupSelector AgentAndGroupSelector;
        private GroupSequence focusedGroup;

        public MeshRenderer HighlighterObject;

        private readonly List<GameObject> memberHighlighters = new List<GameObject>(12);
        private readonly List<PathHighlighter> pathHighlighters = new List<PathHighlighter>(20);

        void Start()
        {
            AgentAndGroupSelector.ObserveGroup(newFocus =>
            {
                ClearAllHighlighters();

                if ((focusedGroup = newFocus) != null)
                {
                    HighlighterObject.enabled = true;

                    foreach (var agent in focusedGroup.agents)
                    {
                        memberHighlighters.Add(
                            Instantiate(HighlighterObject.gameObject, agent.transform, false)
                        );
                    }

                    HighlighterObject.enabled = false;

                    ConfigurePathHighlighterTo(focusedGroup);
                }
            });
        }

        void Update()
        {
            if (focusedGroup != null)
            {
                for (var i = 0; i < focusedGroup.agents.Count - 1; i++)
                {
                    pathHighlighters[i].Update();
                }
            }
        }

        private void ClearAllHighlighters()
        {
            memberHighlighters.ForEach(Destroy);
            memberHighlighters.Clear();

            pathHighlighters.ForEach(p => p.Disable());
        }

        private void ConfigurePathHighlighterTo(GroupSequence group)
        {
            var agents = group.agents.Where(agent => agent != AgentAndGroupSelector.FocusedAgent).ToArray();
            
            while (agents.Length > pathHighlighters.Count)
            {
                pathHighlighters.Add(new PathHighlighter());
            }

            for (var i = 0; i < agents.Length; i++)
            {
                pathHighlighters[i].Initialize(agents[i], transform);
            }
        }
    }
}