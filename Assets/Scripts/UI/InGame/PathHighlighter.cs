using System.Collections.Generic;
using Simulation;
using UnityEngine;

namespace UI.InGame
{
    public class PathHighlighter
    {
        private Agent agent;
        private Transform parent;
        private readonly List<GameObject> pathHighlighters = new List<GameObject>();
        private int enabledHighlighterCount;

        public void Initialize(Agent agent, Transform parent)
        {
            this.parent = parent;
            this.agent = agent;

            Disable();
            Update();
        }

        public void Update()
        {
            if (!agent.HasPath()) return;
            
            var corners = agent.GetPathCorners();
            var requiredHighlighterCount = corners.Length - 1;

            while (requiredHighlighterCount > enabledHighlighterCount && enabledHighlighterCount < pathHighlighters.Count)
            {
                pathHighlighters[enabledHighlighterCount].SetActive(true);
                enabledHighlighterCount++;
            }

            while (requiredHighlighterCount > pathHighlighters.Count)
            {
                pathHighlighters.Add(
                    HighlightLine.CreateLine(
                        Vector3.zero,
                        Vector3.zero,
                        parent
                    )
                );

                enabledHighlighterCount++;
            }

            for (var i = 1; i < corners.Length; i++)
            {
                HighlightLine.UpdateLine(
                    pathHighlighters[i - 1],
                    corners[i - 1],
                    corners[i]
                );
            }
            
            while (enabledHighlighterCount > requiredHighlighterCount)
            {
                pathHighlighters[enabledHighlighterCount - 1].SetActive(false);
                enabledHighlighterCount--;
            }
        }

        public void Disable()
        {
            pathHighlighters.ForEach(o => o.SetActive(false));
            enabledHighlighterCount = 0;
        }
    }
}