using Control;
using Simulation;
using UnityEngine;

namespace UI
{
    public class AgentHighlighter : MonoBehaviour
    {
        public AgentSelector AgentSelector;
        private Agent focusedAgent;

        public MeshRenderer HighlighterObject;

        private readonly PathHighlighter highlighter = new PathHighlighter();
    
        void Start()
        {
            AgentSelector.Observe(newFocus =>
            {
                highlighter.Disable();

                if ((focusedAgent = newFocus) != null)
                {
                    highlighter.Initialize(focusedAgent, transform);
                    
                    HighlighterObject.enabled = true;
                    HighlighterObject.transform.SetParent(focusedAgent.transform, false);
                }
                else
                {
                    HighlighterObject.enabled = false;
                    HighlighterObject.transform.SetParent(transform, false);
                }
            });
        }

        private void Update()
        {
            if (focusedAgent != null)
            {
                highlighter.Update();
            }
        }
    }
}
