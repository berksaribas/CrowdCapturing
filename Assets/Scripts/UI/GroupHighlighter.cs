using System.Collections.Generic;
using System.Linq;
using Control;
using Simulation;
using UnityEngine;
using UnityEngine.AI;

namespace UI
{
    public class GroupHighlighter : MonoBehaviour
    {
        public AgentSelector AgentSelector;
        public GroupSelector GroupSelector;
        private GroupSequence focusedGroup;

        public MeshRenderer HighlighterObject;

        private readonly List<GameObject> memberHighlighters = new List<GameObject>(12);
        private readonly List<PathHighlighter> pathHighlighters = new List<PathHighlighter>(20);

        void Start()
        {
            GroupSelector.Observe(newFocus =>
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
            var agents = group.agents.Where(agent => agent != AgentSelector.FocusedAgent).ToArray();
            
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