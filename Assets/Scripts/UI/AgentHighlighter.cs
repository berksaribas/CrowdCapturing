using Control;
using Simulation;
using UnityEngine;

namespace UI
{
    public class AgentHighlighter : MonoBehaviour
    {
        public AgentAndGroupSelector AgentAndGroupSelector;
        private Agent focusedAgent;

        public MeshRenderer HighlighterObject;

        private readonly PathHighlighter highlighter = new PathHighlighter();
    
        void Start()
        {
            AgentAndGroupSelector.ObserveAgent(newFocus =>
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
