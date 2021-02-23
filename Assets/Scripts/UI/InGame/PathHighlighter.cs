using System.Collections.Generic;
using Simulation;
using UnityEngine;

namespace UI.InGame
{
    public class PathHighlighter : MonoBehaviour
    {
        private PathTraverser[] focusedPathTraversers;
        private readonly List<SinglePathHighlighter> pathHighlighters = new List<SinglePathHighlighter>(20);

        private void Start()
        {
            UIState.PathTraversers.OnChange += newPathTraversers =>
            {
                if ((focusedPathTraversers = newPathTraversers) != null)
                {
                    for (var i = pathHighlighters.Count; i < focusedPathTraversers.Length; i++)
                        pathHighlighters.Add(new SinglePathHighlighter());
                    
                    for (var i = 0; i < focusedPathTraversers.Length; i++)
                        pathHighlighters[i].Initialize(focusedPathTraversers[i], transform);

                    for (var i = focusedPathTraversers.Length; i < pathHighlighters.Count; i++)
                        pathHighlighters[i].Disable();
                }
                else
                {
                    foreach (var pathHighlighter in pathHighlighters)
                        pathHighlighter.Disable();
                }
            };
        }
    }

    internal class SinglePathHighlighter
    {
        private readonly List<GameObject> pathHighlighters = new List<GameObject>();
        private int enabledHighlighterCount;

        public void Initialize(PathTraverser pathTraverser, Transform parent)
        {
            var corners = pathTraverser.Corners;
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
                        parent,
                        0.6f
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