using System.Collections.Generic;
using Control;
using Simulation;
using UnityEngine;
using UnityEngine.AI;

namespace UI
{
    public class AgentHighlighter : MonoBehaviour
    {
        public AgentSelector AgentSelectorObject;
        private Agent focusedAgent;

        public MeshRenderer HighlighterObject;

        public GameObject PathHighlighterPrefab;
        private readonly List<GameObject> pathHighlighter = new List<GameObject>();
    
        // Start is called before the first frame update
        void Start()
        {
            AgentSelectorObject.Observe(newFocus =>
            {
                if ((focusedAgent = newFocus) != null)
                {
                    HighlighterObject.enabled = true;
                    ClearPathHighlighter();
                    ConfigurePathHighlighterTo(focusedAgent);
                }
                else
                {
                    HighlighterObject.enabled = false;
                    ClearPathHighlighter();
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (focusedAgent != null)
            {
                HighlighterObject.transform.position = focusedAgent.transform.position;
            }
        }
    
        private void ClearPathHighlighter()
        {
            pathHighlighter.ForEach(Destroy);
            pathHighlighter.Clear();
        }

        private void ConfigurePathHighlighterTo(Agent agent)
        {
            var path = agent.GetComponent<NavMeshAgent>().path.corners;

            for (var i = 1; i < path.Length; i++)
            {
                pathHighlighter.Add(
                    HighlightLine.CreateNew(
                        path[i - 1],
                        path[i],
                        transform
                    )
                );
            }
        }
    }
}
